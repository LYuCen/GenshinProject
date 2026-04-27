Shader "Weapon/Scope_Final_Professional"
{
    Properties
    {
        [Header(Base Scope View)]
        _MainTex ("Scope RT", 2D) = "white" {}
        _Zoom ("Magnification (Zoom)", Range(0.1, 5.0)) = 1.0
        _MeshSize ("Mesh Manual Scale", Float) = 0.1
        _Aspect ("Aspect Fix (People Flattening)", Range(0.5, 3.0)) = 1.0

        [Header(Reticle Settings)]
        _ReticleTex ("Reticle Texture (White)", 2D) = "white" {}
        _ReticleColor ("Reticle Color", Color) = (1, 0, 0, 1)
        _ReticleBrightness ("Reticle Brightness", Range(0, 10)) = 1.0
        _ReticleSize ("Reticle Size (Adjust this!)", Range(0.001, 0.5)) = 0.05
        _ReticleAspect ("Reticle Aspect (W/H Fix)", Range(0.5, 2.0)) = 1.0
        _ReticleOffset ("Reticle Offset (XY)", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 scopeUV : TEXCOORD0;
                float2 reticleUV : TEXCOORD1;
            };

            sampler2D _MainTex, _ReticleTex;
            float _Zoom, _MeshSize, _Aspect, _ReticleSize, _ReticleAspect, _ReticleBrightness;
            float4 _ReticleColor, _ReticleOffset;

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                
                // --- 1. 背景画面 (Scope View) UV 计算 ---
                float2 bUV = input.positionOS.xy;
                // 修正背景画面的比例，防止看扁
                bUV.x *= _Aspect; 
                output.scopeUV = (bUV * _MeshSize / _Zoom) + 0.5;

                // --- 2. 分划图 (Reticle) UV 计算 ---
                float2 rUV = input.positionOS.xy;
                // 修正准星的比例，防止圆圈变椭圆
                rUV.x *= _ReticleAspect;
                // 核心：分划图大小调节。注意：_ReticleSize 越小，采样越集中，视觉上越大
                // 这里用 (rUV / _ReticleSize) 来实现物理大小控制
                output.reticleUV = (rUV / _ReticleSize) - _ReticleOffset.xy + 0.5;

                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // 采样背景
                half4 scopeCol = tex2D(_MainTex, input.scopeUV);

                // 采样分划图
                half4 reticleSample = tex2D(_ReticleTex, input.reticleUV);

                // 边界裁剪：确保准星只显示在它该在的地方
                // 需要贴图 Wrap Mode 设为 Clamp
                float mask = (input.reticleUV.x >= 0 && input.reticleUV.x <= 1 && 
                             input.reticleUV.y >= 0 && input.reticleUV.y <= 1) ? 1.0 : 0.0;

                half3 reticleRGB = reticleSample.rgb * _ReticleColor.rgb * _ReticleBrightness;
                float alpha = reticleSample.a * _ReticleColor.a * mask;

                // 最终混合
                half3 finalRGB = lerp(scopeCol.rgb, reticleRGB, alpha);

                return half4(finalRGB, 1.0);
            }
            ENDHLSL
        }
    }
}