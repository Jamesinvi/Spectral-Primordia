Shader "Spectral/Ocean"
{
    Properties
    {
        _ShallowColor("Shallow Water", Color) = (0.2,0.5,0.8,1)
        _DeepColor ("Deep Water", Color) = (0.0,0.1,0.4,1)
        _MaxDepth ("Max Water Depth", Float) = 10
        [Header(Normals)][Space]
        [Normal][NoScaleOffset] _NormalA("NormalA", 2D) = "bump"{}
        [Normal][NoScaleOffset] _NormalB ("NormalB", 2D) = "bump"{}
        _NormalStrengthA ("Normal Strength A", Float) = 1
        _NormalStrengthB ("Normal Strength B", Float) = 1
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        [Header(Triplanar)][Space]
        _TriplanarScale ("Tri-planar Scale", Range(0,10)) = 1
        _TriplanarBlendSharpness ("Blend Sharpness", Float) =1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        } ZWrite On Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Tags
            {
                "LightMode"="Unlit"
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.hlsl"
            #include "SpectralCore.hlsl"
            #include "SpectralTransforms.hlsl"
            #include "Triplanar.hlsl"
            TEXTURE2D(_CameraDepth);
            SAMPLER(sampler_CameraDepth);

            CBUFFER_START(UnityPerMaterial)
                real4 _ShallowColor;
                real4 _DeepColor;
                real _MaxDepth;
                real _TriplanarScale;
                real _TriplanarBlendSharpness;
                real _NormalStrengthA;
                real _NormalStrengthB;
                real _Smoothness;
                sampler2D _NormalA;
                sampler2D _NormalB;
                real4 _NormalA_ST;
            CBUFFER_END

            struct Attributes
            {
                real4 posOS : POSITION;
                real3 normOS : NORMAL;
                real4 tanOS : TANGENT;
                real2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                real4 posCS : SV_POSITION;
                real2 uv : TEXCOORD0;
                real4 screenPos : TEXCOORD1;
                real3 posWS : TEXCOORD2;
                real3 normWS : TEXCOORD3;
                real4 tanWS : TEXCOORD4;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.posCS = TransformObjectToHClip(IN.posOS);
                OUT.screenPos = ComputeScreenPos(OUT.posCS);
                OUT.posWS = TransformObjectToWorld(IN.posOS);
                OUT.normWS = TransformObjectToWorldNormal(IN.normOS);
                OUT.tanWS.xyz = TransformObjectToWorldDir(IN.tanOS.xyz);
                OUT.tanWS.w = IN.tanOS.w;
                OUT.uv = IN.uv;
                return OUT;
            }

            real4 frag(Varyings IN) : SV_TARGET
            {
                real3 normWS = normalize(IN.normWS);

                real3 nA = TriplanarNormalMap(IN.posWS, normWS, _NormalA, _NormalStrengthA, _TriplanarScale, _TriplanarBlendSharpness, _Time.x);
                real3 nB = TriplanarNormalMap(IN.posWS, normWS, _NormalB, _NormalStrengthB, _TriplanarScale, _TriplanarBlendSharpness, -_Time.x);
                // merge normal textures
                real3 normTS = normalize(nA + nB);

                real2 dUV = IN.screenPos.xy / IN.screenPos.w;
                real rawTerrain = SAMPLE_DEPTH_TEXTURE(_CameraDepth, sampler_CameraDepth, dUV);
                real terrainVSZ = LinearEyeDepth(rawTerrain, _ZBufferParams);
                real waterVSZ = -mul(UNITY_MATRIX_V, real4(IN.posWS, 1)).z;
                real thickness = saturate((terrainVSZ - waterVSZ) / _MaxDepth);
                real sqrtThickness = sqrt(thickness);
                real4 albedo = lerp(_ShallowColor, _DeepColor, sqrtThickness);
                
                real3 tanWS = normalize(IN.tanWS.xyz);
                real3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.posWS.xyz);
                real4 smoothness = _Smoothness;
                // Sample and unpack the normal map (gives a tangent-space normal)
                real3x3 TBN = CreateTangentToWorld(normWS, tanWS, IN.tanWS.w); // Tangent-bitangent-normal
                normWS = TransformTangentToWorld(normTS, TBN, true);

                real3 reflection = reflect(-viewDir, IN.normWS); // Direction of ray after hitting the surface of object
                real lod = Remap(0, 1, 8, 0, smoothness);
                real3 reflectionCol = PLATFORM_SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflection, lod);


                for (int id = 0; id < min(unity_LightData.y, 4); id++)
                {
                    int i = unity_LightIndices[0][id];
                    ShadingInfo info;
                    info.posWS = IN.posWS;
                    info.normWS = normWS;
                    info.viewDir = viewDir;
                    info.albedo = albedo;
                    info.smoothness = smoothness;
                    info.lightType = _LightColors[i].w;
                    info.lightData = _LightData[i];
                    info.lightCol = _LightColors[i];
                    info.reflectionCol = reflectionCol;
                    albedo += LightContribution(info, .1);;
                }

                for (int id2 = 4; id2 < min(unity_LightData.y, 8); id2++)
                {
                    int i = unity_LightIndices[1][id2 - 4];
                    ShadingInfo info;
                    info.posWS = IN.posWS;
                    info.normWS = normWS;
                    info.viewDir = viewDir;
                    info.albedo = albedo;
                    info.smoothness = smoothness;
                    info.lightType = _LightColors[i].w;
                    info.lightData = _LightData[i];
                    info.lightCol = _LightColors[i];
                    info.reflectionCol = reflectionCol;
                    albedo += LightContribution(info, .1);
                }
                return albedo;
            }
            ENDHLSL
        }
    }
}