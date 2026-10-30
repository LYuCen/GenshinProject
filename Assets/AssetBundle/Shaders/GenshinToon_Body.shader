Shader "GenshinToon/Body"
{
    Properties
    {
        [Header(Textures)]
        _BaseMap ("Base Map", 2D) = "white"{} 
        _LightMap ("Light Map", 2D) = "white"{} 
        [Toggle(_USE_LIGHTMAP_AO)] _UseLightMapAO ("Use LightMap AO", Range(0, 1)) = 1 
        _RampTex ("Ramp Tex", 2D) = "white"{} 
        _RampWeight ("Ramp Influence Weight", Range(0, 1)) = 1.0
        _ShadowRampWidth ("Shadow Ramp Width", Float) = 1 
        _ShadowPosition ("Shadow Position", Float) = 0.55 
        _ShadowSoftness ("Shadow Softness", Float) = 0.5 
        
        [Toggle] _UseRampShadow2 ("Use Ramp Shadow 2", Range(0, 1)) = 1 
        [Toggle] _UseRampShadow3 ("Use Ramp Shadow 3", Range(0, 1)) = 1 
        [Toggle] _UseRampShadow4 ("Use Ramp Shadow 4", Range(0, 1)) = 1 
        [Toggle] _UseRampShadow5 ("Use Ramp Shadow 5", Range(0, 1)) = 1 
        
        // 角色影子
        [Header(Lighting Options)]
        _DayOrNight ("Day Or Night", Range(0, 1)) = 0
        
        // 边缘光
        [Header(Rim Light)]
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.5, 10)) = 5.0
        
        // 金属光
        [Header(Matal Light)]
        _SpecColor ("SpecColor", Color) = (1, 1, 1, 1)
        _SpecSize ("Specular Size", Range(0, 1)) = 0.5
        _SpecIntensity ("Specular Intensity", Range(0, 10)) = 1.0
        
        // Blinn Phong 高光
        [Header(Blinn Phong Specular)]
        _BP_Color ("BP Color", Color) = (1, 1, 1, 1)
        _Gloss ("Glossiness", Range(1, 256)) = 32.0 // 值越大，光点越小越集中
        _BP_Intensity ("BP Intensity", Range(0, 5)) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline" // 修正标签
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);    SAMPLER(sampler_BaseMap);
            TEXTURE2D(_LightMap);   SAMPLER(sampler_LightMap);
            TEXTURE2D(_RampTex);    SAMPLER(sampler_RampTex);
        
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _LightMap_ST;
                float4 _RampTex_ST;

                float _RampWeight;
                float _ShadowRampWidth;
                float _ShadowPosition; // 修正了拼写错误 Positon -> Position
                float _ShadowSoftness;
                float _UseRampShadow2;
                float _UseRampShadow3;
                float _UseRampShadow4;
                float _UseRampShadow5;
                float _DayOrNight;

                // 边缘光
                float4 _RimColor;
                float _RimPower;

                float4 _SpecColor;
                float _SpecSize;
                float _SpecIntensity;

                float4 _BP_Color;
                float _Gloss;
                float _BP_Intensity;
            CBUFFER_END

            // 计算 Ramp 的 V 坐标（行号）
            float RampShadowID(float input, float useShadow2, float useShadow3, float useShadow4, float useShadow5)
            {
                float v1 = step(0.6, input) * step(input, 0.8);  
                float v2 = step(0.4, input) * step(input, 0.6);
                float v3 = step(0.2, input) * step(input, 0.4);
                float v4 = step(input, 0.2);

                float blend12 = lerp(1, 2, useShadow2);
                float blend13 = lerp(1, 3, useShadow3);
                float blend14 = lerp(1, 4, useShadow4);
                float blend15 = lerp(1, 5, useShadow5);

                float result = blend12;
                result = lerp(result, blend15, v1); 
                result = lerp(result, blend13, v2);
                result = lerp(result, blend14, v3);
                result = lerp(result, 1.0, v4);

                return result;
            }

            struct UniversalAttributes
            {
                float4 positionOS : POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 normalOS : NORMAL;
                float4 color : COLOR; // 顶点颜色
            };

            struct UniversalVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 color : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
            };
        
            UniversalVaryings MainVS(UniversalAttributes input)
            {
                UniversalVaryings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;

                VertexNormalInputs vertexNormalInputs = GetVertexNormalInputs(input.normalOS);
                output.normalWS = vertexNormalInputs.normalWS;
                
                output.uv0 = input.uv0;
                output.color = input.color;
                return output;
            }

            half4 MainFS(UniversalVaryings input) : SV_TARGET
            {
                Light light = GetMainLight();
                half4 vertexColor = input.color;

                half3 N = normalize(input.normalWS);
                half3 L = normalize(light.direction);
                half dotNL = dot(N, L); 
                
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv0);
                half4 lightMap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, input.uv0);

                half halfLambert = dotNL * 0.5 + 0.5;
                // 使用乘法代替 pow(x, 2) 优化性能
                half shadowFactor = halfLambert * halfLambert;
                half lamertstep = smoothstep(0.01, 0.4, shadowFactor); 
                shadowFactor = lerp(0, shadowFactor, lamertstep); 
                
                // AO
                #if _USE_LIGHTMAP_AO
                    half ambient = lightMap.g; 
                #else
                    half ambient = halfLambert;
                #endif
                    
                half shadow = (ambient + halfLambert) * 0.5;
                shadow = lerp(shadow, 1, step(0.98, ambient));
                shadow = lerp(shadow, 0, step(ambient, 0.02));

                half isShadowArea = step(shadow, _ShadowPosition); 
                half shadowDepth = saturate((_ShadowPosition - shadow) / _ShadowPosition); 
                shadowDepth = pow(max(0.0001, shadowDepth), _ShadowSoftness); 
                
                half rampWidthFactor = vertexColor.g * 2 * _ShadowRampWidth; 
                
                // Ramp 色阶图逻辑
                half rampU = 1 - saturate(shadowDepth / max(0.0001, rampWidthFactor)); 
                half rampID = RampShadowID(lightMap.a, _UseRampShadow2, _UseRampShadow3, _UseRampShadow4, _UseRampShadow5);
                
                half rampV = 0.45 - (rampID - 1.0) * 0.1;
                
                half2 rampDayUV = half2(rampU, rampV + 0.5); 
                half3 rampDayColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, rampDayUV).rgb;
                
                half2 rampNightUV = half2(rampU, rampV); 
                half3 rampNightColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, rampNightUV).rgb;
                
                half3 rampColor = lerp(rampDayColor, rampNightColor, _DayOrNight); 

                half3 toonResult = baseMap.rgb * rampColor * (isShadowArea ? 1.0 : 1.2);
                // 方案 2: 基础 Lambert 结果 (Fallback)
                half3 defaultResult = baseMap.rgb * halfLambert * (shadow + 0.2);

                half3 finalColor;
                #if _USE_RAMP_SHADOW
                    finalColor = lerp(defaultResult, toonResult, _RampWeight);
                #else
                    finalColor = defaultResult;
                #endif

                // 边缘光
                half3 V = normalize(_WorldSpaceCameraPos.xyz - input.positionWS); // 计算视线方向
                half fresnel = pow(1.0 - saturate(dot(N, V)), _RimPower); // 计算菲涅尔值
                half3 materialRim = baseMap.rgb * _RimColor.rgb * fresnel; // 根据像素颜色处理边缘光颜色
                finalColor += materialRim; // 将边缘光加到原颜色上

                // 金属高光
                half3 metalV = normalize(_WorldSpaceCameraPos.xyz - input.positionWS); 
                half3 metalH = normalize(L + metalV);
                half dotNH = saturate(dot(N, metalH));
                
                // 高光范围
                half specThreshold = 1.0 - (_SpecSize * 0.1); // 将 0-1 映射到合适的阈值
                half specShape = pow(dotNH, 20.0);
                half specular = smoothstep(0.5, 0.52, specShape); 

                half specularMask = lightMap.r; // 确保你的贴图 R 通道有白色区域
                
                half3 finalSpecular = _SpecColor.rgb * specular * specularMask * _SpecIntensity;

                finalColor += finalSpecular;

                // Blinn-Phong 高光
                half3 H_BP = normalize(L + V); // 半角向量
                half dotNH_BP = saturate(dot(N, H_BP));

                // 标准 Blinn-Phong 公式：计算指数幂
                // _Gloss 越大，高光越聚拢；_Gloss 越小，高光越弥散
                half bpSpec = pow(dotNH_BP, _Gloss);

                half bpMask = lightMap.b;

                // 结合灯光颜色和强度
                half3 finalBP = _BP_Color.rgb * light.color * bpSpec * _BP_Intensity * bpMask;
                
                finalBP *= (isShadowArea ? 0.0 : 1.0); 

                finalColor += finalBP;

                
                return half4(finalColor, 1.0);
            }
        ENDHLSL
        
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
                #pragma vertex MainVS
                #pragma fragment MainFS

                // 修正变体指令，增加下划线支持“关闭”状态
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile_fragment _ _SHADOWS_SOFT
                #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

                #pragma multi_compile_local _ _USE_LIGHTMAP_AO
                #pragma multi_compile_local _ _USE_RAMP_SHADOW

                
            ENDHLSL
        }

        Pass
        {
            Name "UniversalForward"
            Tags {
                "LightMode" = "SRPDefaultUnlit"
                "Queue" = "Geometry+1" // 渲染队列位置后一位
            }
            
            Cull Front
            
            HLSLPROGRAM
                #pragma vertex MainVS
                #pragma fragment MainFS

                // 修正变体指令，增加下划线支持“关闭”状态
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile_fragment _ _SHADOWS_SOFT
                #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

                #pragma multi_compile_local _ _USE_LIGHTMAP_AO
                #pragma multi_compile_local _ _USE_RAMP_SHADOW

                UniversalVaryings BackMainVS(UniversalAttributes input)
                {
                    UniversalVaryings output = MainVS(input);
                    output.uv0 = input.uv1;
                    return output;
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
                #pragma multi_compile _ DOTS_INSTANCING_ON // 启用 DOTS 实例化渲染
                #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW // 启用点光源阴影

                #pragma vertex ShadowVS
                #pragma fragment ShadowFS

                // 编译器自己赋值
                float3 _LightDirection; // 光源方向
                float3 _LightPosition; // 光源位置

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                };

                // 将阴影的世界空间顶点位置转换为适合阴影投射的裁剪空间的位置
                float4 GetShadowPositionHClip(Attributes input)
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

                Varyings ShadowVS(Attributes input)
                {
                    Varyings output;
                    output.positionCS = GetShadowPositionHClip(input);
                    return output;
                }

                half4 ShadowFS(Varyings input) : SV_TARGET
                {
                    return 0;
                }
                
            ENDHLSL
        }
    }
}