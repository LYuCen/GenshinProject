Shader "GenshinToon/AmbientOcclusion" // 外部添加 shader 的路径
{
    Properties // 外部可见属性 可以在编辑器调整
    {
        [Header(Textures)]
        _BaseMap ("Base Map", 2D) = "white"{} // 基础纹理
        _LightMap ("Light Map", 2D) = "white"{} // 光照贴图
        [Toggle(_USE_LIGHTMAP_AO)] _UseLightMapAO ("Use LightMap AO", Range(0,1)) = 1 // 环境光遮蔽开关
    }
    SubShader // 子着色器 (可包含多个子着色器)
    {
        Tags    // 标签
        {
            "RenderPipeline" = "UniversalPipeline" // 指定渲染管线为 URP
            "RenderType" = "Opaque" // 指定渲染类型为不透明
        }
        
        HLSLINCLUDE // 公共代码块开始
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" // 核心库
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl" // 光照库
        
            // 放预处理指令、头文件、常量定义、函数定义
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS // 主光源阴影
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE // 主光源阴影级联
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN // 主光源阴影屏幕空间

            #pragma multi_compile_fragment _ _LIGHT_LAYERS // 光照层
            #pragma multi_compile_fragment _ _LIGHT_COOKIES // 光照饼干
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION // 屏幕空间遮挡
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS // 额外光源阴影
            #pragma multi_compile_fragment _ _SHADOWS_SOFT // 阴影软化

            #pragma multi_compile_local _USE_LIGHTMAP_AO // AO 开关
    
            TEXTURE2D(_BaseMap);    SAMPLER(sampler_BaseMap);
            TEXTURE2D(_LightMap);   SAMPLER(sampler_LightMap);
        
            CBUFFER_START(UnityPerMaterial) // 常量缓冲区
                float4 _BaseMap_ST;
                float4 _LightMap_ST;
                float _UseLightMapAO;
            CBUFFER_END
        ENDHLSL // 公共代码块结束
        
        Pass // 渲染通道
        {
            Name "UniversalForward" // 通道名称
            Tags
            {
                "LightMode" = "UniversalForward" // 光照模式 ： 向前渲染
            }
            
            HLSLPROGRAM // 着色器程序开始
                
                #pragma vertex MainVS   // 声明顶点着色器函数
                #pragma fragment MainFS // 声明片元着色器函数

                // 顶点着色器顶点信息
                struct UniversalAttributes
                {
                    float4 positionOS : POSITION; // 本地空间顶点坐标
                    float2 uv0 : TEXCOORD0; // 第一套纹理坐标
                    float3 normalOS : NORMAL; // 本地坐标法线
                };

                // 片元着色器输入信息
                struct UniversalVaryings
                {
                    float4 positionCS : SV_POSITION; // 返回裁剪空间坐标
                    float2 uv0 : TEXCOORD0;
                    float3 normalWS : TEXCOORD1; // 世界空间法线
                };
            
                // 顶点着色器 : 返回裁剪空间坐标
                // 指定传入参数为 POSITION
                UniversalVaryings MainVS(UniversalAttributes input)
                {
                    UniversalVaryings output; // 定义顶点着色器返回值
                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz); // 转换顶点空间坐标
                    output.positionCS = vertexInput.positionCS; // 转换为裁剪空间坐标

                    VertexNormalInputs vertex_normal_inputs = GetVertexNormalInputs(input.normalOS); // 转换法线空间坐标
                    output.normalWS = vertex_normal_inputs.normalWS; // 拿到世界空间法线
                    
                    output.uv0 = input.uv0; // 拿到纹理坐标
                    return output;
                }
                // 片元着色器 : 返回颜色 (rgba)
                // 函数后 : 返回颜色
                half4 MainFS(UniversalVaryings input) : SV_TARGET
                {
                    Light light = GetMainLight();

                    // 归一化
                    half3 N = normalize(input.normalWS);
                    half3 L = normalize(light.direction);
                    half Not = dot(N, L); // 计算法线和光源方向的点积
                    
                    half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv0); // 采样纹理贴图
                    half4 lightMap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, input.uv0); // 采样光照贴图

                    half lambert = Not; // 兰伯特光照 (-1 ~ 1)
                    half halfLambert = lambert * 0.5 + 0.5; // 半兰伯特 (0 ~ 1)
                    half halfLambertSq = halfLambert * halfLambert;

                    // AO
                    #if _USE_LIGHTMAP_AO
                        half ambient = lightMap.g; // 环境光
                    #else
                        half ambient = halfLambert;
                    #endif
                        
                    half shadow = (ambient + halfLambert) * 0.5; // 环境光遮蔽加上半兰伯特
                    shadow = lerp(shadow, 1, step(0.98, ambient));
                    shadow = lerp(shadow, 0, step(ambient, 0.02));

                    half3 finalColor = baseMap.rgb * halfLambertSq * (shadow + 0.2); // 获取最终颜色
                    
                    return half4(finalColor.rgb,1);
                }
            ENDHLSL
        }
    }
}
