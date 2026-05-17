Shader "URP/Character_Aura_Field"
{
    Properties
    {
        [HDR] _AuraColor ("Aura Color", Color) = (0, 1, 0.5, 1)
        _AuraSize ("Aura Size (Distance)", Range(0, 0.5)) = 0.05    // 气场离开身体的距离
        _RimPower ("Aura Softness", Range(0.1, 10)) = 3.0          // 气场的透明度渐变
        _AuraIntensity ("Intensity", Range(0, 10)) = 2.0           // 亮度
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent+2" // 确保渲染在最外层
            "RenderPipeline" = "UniversalPipeline" 
        }

        // 使用加法混合 (One One) 更有“能量感”
        // 或者使用标准混合 (SrcAlpha OneMinusSrcAlpha)
        Blend SrcAlpha One
        ZWrite Off
        Cull Back 

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct UniversalAttributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct UniversalVaryings
            {
                float4 positionCS   : SV_POSITION;
                float3 viewDirWS    : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _AuraColor;
                float _AuraSize;
                float _RimPower;
                float _AuraIntensity;
            CBUFFER_END

            UniversalVaryings vert(UniversalAttributes IN)
            {
                UniversalVaryings OUT;
                
                // 1. 获取世界坐标和法线
                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normWS = TransformObjectToWorldNormal(IN.normalOS);
                
                // 2. 【核心逻辑】沿着法线方向把顶点往外推
                // 这样气场就离开了身体
                posWS += normWS * _AuraSize;
                
                OUT.viewDirWS = GetWorldSpaceViewDir(posWS);
                OUT.normalWS = normWS;
                OUT.positionCS = TransformWorldToHClip(posWS);
                
                return OUT;
            }

            half4 frag(UniversalVaryings IN) : SV_Target
            {
                float3 normal = normalize(IN.normalWS);
                float3 viewDir = normalize(IN.viewDirWS);

                // 计算边缘光
                float fresnel = 1.0 - saturate(dot(normal, viewDir));
                
                // 让气场中心透明，边缘亮
                float aura = pow(fresnel, _RimPower);

                float4 finalColor = _AuraColor;
                // Alpha 取决于边缘光强度和亮度设置
                finalColor.a *= aura * _AuraIntensity;

                // 如果使用 Blend One One，颜色越暗的地方越透明
                return finalColor;
            }
            ENDHLSL
        }
    }
}