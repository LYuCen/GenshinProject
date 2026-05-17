Shader "Weapon/SelfLight"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _EmissiveColor ("Emissive Color", Color) = (1, 0.5, 0, 1)
        _EmissiveIntensity ("Intensity", Range(0, 5)) = 1
    }
    
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _EmissiveColor;
                float _EmissiveIntensity;
            CBUFFER_END
            
            struct UniversalAttributes
            {
                float4 positionOS : POSITION;
            };
            
            struct UniversalVaryings
            {
                float4 positionCS : SV_POSITION;
            };
            
            UniversalVaryings Vert(UniversalAttributes input)
            {
                UniversalVaryings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }
            
            half4 Frag(UniversalVaryings input) : SV_TARGET
            {
                // 基础颜色 + 自发光颜色
                half3 finalColor = _Color.rgb + (_EmissiveColor.rgb * _EmissiveIntensity);
                return half4(finalColor, _Color.a);
            }
            ENDHLSL
        }
    }
}