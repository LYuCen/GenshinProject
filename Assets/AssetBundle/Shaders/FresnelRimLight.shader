Shader "GenshinToon/FresnelRimLight"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _RimColor("Rim Color", Color) = (0, 0.5, 1, 1)
        _RimPower("Rim Power", Range(0.5, 10.0)) = 3.0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct UniversalAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct UniversalVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 viewDirWS  : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _RimColor;
                float _RimPower;
            CBUFFER_END

            UniversalVaryings vert(UniversalAttributes input)
            {
                UniversalVaryings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                
                return output;
            }

            half4 frag(UniversalVaryings input) : SV_Target
            {
                float3 N = normalize(input.normalWS);
                // 强制计算视线方向
                float3 V = normalize(_WorldSpaceCameraPos.xyz - input.viewDirWS); 

                // 直接输出菲涅尔计算结果作为颜色
                float fresnel = pow(1.0 - saturate(dot(N, V)), _RimPower);
                
                // 如果 Shader 正常工作，你应该看到一个黑色的剪影，边缘有强烈的蓝色
                return float4(_RimColor.rgb * fresnel * 2.0, 1.0); 
            }
            ENDHLSL
        }
    }
}