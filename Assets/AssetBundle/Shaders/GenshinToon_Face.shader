Shader "GenshinToon/Face"
{
    Properties
    {
        [Header(Textures)]
        _BaseMap ("Base Map", 2D) = "white"{} 
        
        [Header(Shadow Options)]
        [Toggle (_USE_SDF_SHADOW)] _UseSDFShadow ("Use SDF Shadow", Range(0, 1)) = 1 
        _SDF ("SDF", 2D) = "white" {}
        _ShadowMask ("Shadow Mask", 2D) = "white" {} 
        _ShadowColor ("Shadow Color", Color) = (1, 0.87, 0.87, 1)
        _ShadowWeight ("Shadow Intensity", Range(0, 1)) = 1.0
        
        [Header(Head Direction)]
        [HideInInspector] _HeadForward ("Head Forward", Vector) = (0, 0, 1, 0) 
        [HideInInspector] _HeadRight ("Head Right", Vector) = (1, 0, 0, 0) 
        [HideInInspector] _HeadUp ("Head Up", Vector) = (0, 1, 0, 0)
        
        [Header(Face Blush)]
        _FaceBlushColor ("Face Blush Color", Color) = (1, 0, 0, 1)
        _FaceBlushWeight ("Face Blush Weight", Range(0, 1)) = 0
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

            // 1. 定义纹理和采样器（注意拼写：sampler_SDF）
            TEXTURE2D(_BaseMap);     SAMPLER(sampler_BaseMap);
            TEXTURE2D(_SDF);         SAMPLER(sampler_SDF);
            TEXTURE2D(_ShadowMask);  SAMPLER(sampler_ShadowMask);
            
            CBUFFER_START(UnityPerMaterial)
                // 2. 为了支持 SRP Batcher，所有在 Properties 定义的非资源变量都必须放在这里
                // 纹理的缩放偏移（_ST）虽然没用到，但建议带上
                float4 _BaseMap_ST;
                float4 _SDF_ST;
                float4 _ShadowMask_ST;
                float4 _ShadowColor;
                float _ShadowWeight;
                float3 _HeadForward;
                float3 _HeadRight;
                float3 _HeadUp;

                // face blush
                float4 _FaceBlushColor;
                float _FaceBlushWeight;
                
            CBUFFER_END
        ENDHLSL
        
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
                #pragma vertex MainVS
                #pragma fragment MainFS

                // 3. 将 Pragma 指令从 HLSLINCLUDE 移到 Pass 内部，防止多 Pass 干扰
                // 且主光源变体必须带下划线 '_'
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile_fragment _ _ADDITIONAL_LIGHTS
                #pragma multi_compile_fragment _ _SHADOWS_SOFT

                // 4. 使用 shader_feature_local 处理 Toggle 开关
                #pragma multi_compile_local _USE_SDF_SHADOW

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
                    float3 positionWS : TEXCOORD2;
                };
            
                UniversalVaryings MainVS(UniversalAttributes input)
                {
                    UniversalVaryings output;
                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    output.positionCS = vertexInput.positionCS;

                    VertexNormalInputs vertexNormalInputs = GetVertexNormalInputs(input.normalOS);
                    output.normalWS = vertexNormalInputs.normalWS;
                    
                    output.uv0 = input.uv0;
                    return output;
                }

                half4 MainFS(UniversalVaryings input) : SV_TARGET
                {
                    Light light = GetMainLight();

                    half3 N = normalize(input.normalWS);
                    half3 L = normalize(light.direction);
                    
                    // 向量准备
                    half3 headUpDir = normalize(_HeadUp);
                    half3 headForwardDir = normalize(_HeadForward);
                    half3 headRightDir = normalize(_HeadRight);
                    
                    // 采样
                    half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv0);
                    half4 shadowMask = SAMPLE_TEXTURE2D(_ShadowMask, sampler_ShadowMask, input.uv0);

                    #if _USE_SDF_SHADOW
                        // 面部阴影计算逻辑
                        // 计算光照在水平面上的投影
                        half3 LpU = dot(L, headUpDir) * headUpDir; 
                        half3 LpHeadHorizon = normalize(L - LpU); 
                        
                        // 计算夹角控制 SDF 采样
                        half ctrlValue = acos(dot(LpHeadHorizon, headRightDir)) / 3.141592654; 
                        half exposeRight = step(ctrlValue, 0.5); 
                        
                        half valueR = pow(max(0, 1 - ctrlValue * 2), 3); 
                        half valueL = pow(max(0, ctrlValue * 2 - 1), 3); 
                        half mixValue = lerp(valueL, valueR, exposeRight); 

                        // SDF 采样（左右镜像）
                        half sdfLeft = SAMPLE_TEXTURE2D(_SDF, sampler_SDF, half2(1 - input.uv0.x, input.uv0.y)).r;
                        half sdfRight = SAMPLE_TEXTURE2D(_SDF, sampler_SDF, input.uv0).r;
                        half mixSdf = lerp(sdfRight, sdfLeft, exposeRight); 

                        half sdf = step(mixValue, mixSdf); 
                        
                        // 正面判断，防止后脑勺漏光
                        sdf = lerp(0, sdf, step(0, dot(LpHeadHorizon, headForwardDir))); 
                        sdf *= shadowMask.g; 
                        sdf = lerp(sdf, 1, shadowMask.a);

                        half3 targetShadowColor = _ShadowColor.rgb * baseMap.rgb;
                        // Weight=1 是全暗，Weight=0 是没阴影（等于原色）
                        half3 lerpedShadowColor = lerp(baseMap.rgb, targetShadowColor, _ShadowWeight);
                        
                        // 使用混合后的阴影色进行最后的 SDF 插值
                        half3 finalColor = lerp(lerpedShadowColor, baseMap.rgb, sdf);

                        half blushWeight = lerp(0, baseMap.a, _FaceBlushWeight);
                    
                    #else
                        half dotNL = dot(N, L);
                        half halfLambert = dotNL * 0.5 + 0.5;
                        half3 finalColor = baseMap.rgb * (halfLambert * halfLambert) * light.color;
                    #endif

                    finalColor = lerp(finalColor, finalColor *  _FaceBlushColor.rgb, blushWeight);
                    
                    return half4(finalColor, 1.0);
                }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            
            ZWrite On // 写入深度缓冲区
            ZTest LEqual // 速度测试： 小于等于
            ColorMask 0 // 不写入颜色缓冲区
            Cull Off // 不裁剪
            
            HLSLPROGRAM

                #pragma multi_compile_instancing // 启用 GPU 实例化编译
                #pragma mutli_compile _ DOTS_INSTANCING_ON // 启用 DOTS 实例化渲染
                #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW // 启用点光源阴影

                #pragma vertex ShadowVS
                #pragma fragment ShadowFS

                // 编译器自己赋值
                float3 _LightDirection; // 光源方向
                float3 _LightPosition; // 光源位置

                struct UniversalAttributes
                {
                    float4 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                };

                struct UniversalVaryings
                {
                    float4 positionCS : SV_POSITION;
                };

                // 将阴影的世界空间顶点位置转换为适合阴影投射的裁剪空间的位置
                float4 GetShadowPositionHClip(UniversalAttributes input)
                {
                    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                    #if _CASTING_PUNCTUAL_LIGHT_SHADOW // 点光源
                        float3 lightDirectionWS = normalize(_LightPosition - positionWS);
                    #else // 平行光
                        float3 lightDirectionWS = _LightDirection;
                    #endif

                    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

                    #if UNITY_REVERSED_Z
                        positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                    #else
                        positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                    #endif

                    return positionCS;
                }

                UniversalVaryings ShadowVS(UniversalAttributes input)
                {
                    UniversalVaryings output;
                    output.positionCS = GetShadowPositionHClip(input);
                    return output;
                }

                half4 ShadowFS(UniversalVaryings input) : SV_TARGET
                {
                    return 0;
                }
            ENDHLSL
        }
    }
}