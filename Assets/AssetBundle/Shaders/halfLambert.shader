Shader "GenshinToon/HalfLambert"
{
    Properties
    {
        [Header(Textures)]
        _BaseMap ("Base Map", 2D) = "white"{} 
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST; 
            CBUFFER_END
        ENDHLSL
        
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
                #pragma vertex MainVS
                #pragma fragment MainFS

                // --- 只在这里保留必要的变体，且必须带下划线 '_' 以便支持“关闭”状态 ---
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile_fragment _ _SHADOWS_SOFT
                #pragma multi_compile_fragment _ _ADDITIONAL_LIGHTS

                struct UniversalAttributes
                {
                    float4 positionOS : POSITION;
                    float2 uv0 : TEXCOORD0;
                    float3 normalOS : NORMAL;
                };

                struct UniversalVaryings
                {
                    float4 positionCS : SV_POSITION;
                    float2 uv0 : TEXCOORD0;
                    float3 normalWS : TEXCOORD1;
                };
            
                UniversalVaryings MainVS(UniversalAttributes input)
                {
                    UniversalVaryings output;
                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    output.positionCS = vertexInput.positionCS;

                    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS);
                    output.normalWS = vertexNormalInput.normalWS;
                    
                    output.uv0 = input.uv0;
                    return output;
                }

                half4 MainFS(UniversalVaryings input) : SV_TARGET
                {
                    // 获取光照数据
                    Light light = GetMainLight();

                    half3 N = normalize(input.normalWS);
                    half3 L = normalize(light.direction);
                    half dotNL = dot(N, L); 
                    
                    // 采样
                    half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv0);

                    // 半兰伯特计算
                    half halfLambert = dotNL * 0.5 + 0.5;
                    halfLambert = halfLambert * halfLambert;

                    // 最终颜色渲染
                    half3 finalColor = baseMap.rgb * halfLambert;
                    
                    return half4(finalColor, 1.0);
                }
            ENDHLSL
        }
    }
}