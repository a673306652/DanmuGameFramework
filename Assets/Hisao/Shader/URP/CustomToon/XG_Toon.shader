Shader "Hisao/XG_Toon"
{
    Properties
    {
        _BaseMap ("Base Texture",2D) = "white"{}
        _BaseColor("Base Color",Color) = (1,1,1,1)
        _SpecularColor("SpecularColor/反射颜色",Color)=(1,1,1,1)
        _Smoothness("Smoothness/反射强度",float)=10
        _Cutoff("Cutoff/当大于1时裁剪此模型",float)=0.5
        _LightThreshold("LightThreshold/光照阈值",Range(-1,1))=0
        _SmoothLight("SmoothLight/平滑光照",Range(0,1)) = 1
        _LightColor("LightColor/亮部颜色",Color) = (1,1,1,1)
        _DarkColor("DarkColor/暗部颜色",Color) = (0,0,0,0)
        _SkyColor("SkyColor/天光强度",Range(0,1)) = 1
        [Toggle(USE_SHADOW)]_UseShadow("UseShadow/开启阴影",float) = 0
        _ShadowColor("ShadowColor/阴影颜色",Color) = (0,0,0,0)
        _ShadowIntensity("ShadowIntensity/阴影强度",float) = 0
        _LightMaxClamp("LightMaxClamp/限制最亮",Range(0,20)) = 1
        _LightMinClamp("LightMinClamp/限制最暗",Range(0,1)) = 0
        _GlobalBlend("GlobalBlend/整体光照混合",Range(0,1)) = 1
        _EmissionMap("Emission Map/自发光贴图",2D) = "white"{}
        [HDR]_EmissionColor("EmissionColor/自发光",Color) = (0,0,0,0)
        _Shining("Shining",float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry"
            "RenderType"="Opaque"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        CBUFFER_START(UnityPerMaterial)
        float4 _BaseMap_ST;
        float4 _BaseColor;
        float4 _SpecularColor;
        float _Smoothness;
        float _Cutoff;
        float _LightThreshold;
        float _SmoothLight;
        float _SkyColor;
        float4 _LightColor;
        float4 _DarkColor;
        float4 _ShadowColor;
        float _ShadowIntensity;
        float _LightMaxClamp;
        float _LightMinClamp;
        float _GlobalBlend;
        float4 _EmissionMap_ST;
        float4 _EmissionColor;
        CBUFFER_END
        ENDHLSL


        Pass
        {
            Name "URPSimpleLit"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_SHADOW
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);
            float _Shining;

            Varings vert(Attributes IN)
            {
                Varings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS.xyz);
                OUT.positionCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;
                OUT.viewDirWS = GetCameraPositionWS() - positionInputs.positionWS;
                OUT.normalWS = normalInputs.normalWS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }


            half3 Unity_Clamp_float4(half3 In, half3 Min, half3 Max)
            {
                return  clamp(In, Min, Max);
            }

            half3 GetBlingPhong(float GlossValue, half3 ViewDirection, half3 WorldNormalVector, Light MainLight)
            {
                if (GlossValue <= 0)
                {
                    return half3(0, 0, 0);
                }
                half3 dir = normalize(MainLight.direction);
                half3 halfView = normalize(normalize(ViewDirection) + normalize(dir));
                half3 HighLight = max(0, dot(halfView, normalize(WorldNormalVector)));
                HighLight = pow(HighLight, GlossValue);
                return HighLight;
            }

            float GetLambertModel(float lightThreshold, float smoothLight, half3 lightColor, half3 darkColor,
                                  half3 worldNormal, Light light)
            {
                float lambert = RangeRemap(float2(-1, 1), float2(0, 1),
                                           dot(normalize(light.direction), normalize(worldNormal)) + lightThreshold);
                //  float lambert = dot( normalize(light.direction),normalize(worldNormal)) + lightThreshold;
                float smoothLightModel = clamp(pow(clamp(lambert, 0, 1), clamp(smoothLight, 0.001, 999)), 0, 1);
                // half3 col = lerp(darkColor,lightColor,smoothLightModel);
                return smoothLightModel;
            }

            float4 frag(Varings IN):SV_Target
            {
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half4 Emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, IN.uv) * _EmissionColor;
                //计算主光
                Light light = GetMainLight();
                half3 ambient = _GlossyEnvironmentColor.rgb;
                float lightModel = GetLambertModel(_LightThreshold, _SmoothLight, _LightColor, _DarkColor, IN.normalWS,
                                                   light);

                half3 diffuse = lerp(_DarkColor, _LightColor, lightModel);


                #ifdef USE_SHADOW
                float4 SHADOW_COORDS = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(SHADOW_COORDS);

                half3 shadowColor = lerp(1, _ShadowColor, _ShadowIntensity) * diffuse;

                diffuse = lerp(shadowColor, diffuse, mainLight.shadowAttenuation);
                #endif


                half3 specular = GetBlingPhong(_Smoothness, IN.viewDirWS, IN.normalWS, light) * _SpecularColor;

                uint pixelLightCount = GetAdditionalLightsCount();

                for (uint lightIndex = 0; lightIndex < pixelLightCount; ++lightIndex)
                {
                     Light light = GetAdditionalLight(lightIndex, IN.positionWS);
                    diffuse +=  LightingLambert(light.color,light.direction,IN.normalWS) * light.distanceAttenuation;
                    specular += GetBlingPhong(_Smoothness,IN.viewDirWS,IN.normalWS,light)* light.distanceAttenuation;
                }

                baseMap = baseMap* _BaseColor;
                // diffuse = lerp(baseMap, diffuse, _GlobalBlend) + Emission;
                diffuse = lerp( baseMap, (Unity_Clamp_float4(diffuse, half3( _LightMinClamp,_LightMinClamp,_LightMinClamp),half3(_LightMaxClamp,_LightMaxClamp,_LightMaxClamp))  + ambient * _SkyColor + specular) * baseMap,_GlobalBlend) +Emission;


                clip(baseMap.a - _Cutoff);
                return lerp( float4(diffuse, 1),float4(1,1,1,1),_Shining);
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

            ZWrite On
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment


            //由于这段代码中声明了自己的CBUFFER，与我们需要的不一样，所以我们注释掉他
            //#include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            //它还引入了下面2个hlsl文件
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

    }
}