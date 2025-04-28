Shader
"Spectral/PlanetLit"
{
    Properties
    {
        [Header(Colors)]
        _PeakColor("Peaks Color", Color) = (1.0,1.0,1.0,1.0)
        _HighlandsColor("Highlands Color", Color) = (1.0,1.0,1.0,1.0)
        _MidlandsColor("Midlands Color", Color) = (1.0,1.0,1.0,1.0)
        _LowlandsColor("Lowlands Color", Color) = (1.0,1.0,1.0,1.0)
        _WaterBedColor("Water Color", Color) = (1.0,1.0,1.0,1.0)
        [Header(Blending)]
        _SlopeMultiplier("Slope Multiplier", Range(0,5)) =1
        _WaterLevel("Waterbed=>LowLands", Range(0,1)) = 0.1
        _LowToMid("Lowlands=>Midlands", Range(0,1)) = 0.33
        _MidToHigh("Midlands=>Highlands", Range(0,1)) = 0.66
        _HighToPeak("Highlands=>Peaks", Range(0,1)) = 0.90
        _MaxAltitude("MaxAltitude", Float) = 1
        [Header(Normals)]
        [Normal][NoScaleOffset]_Normal("Normal", 2D) = "bump" {}
        _NormalStrength("Normal Strength", Range(0,1)) = 1.0
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        [NoScaleOffset]_SmoothnessTex("Smoothness Map", 2D) = "white"{}
        [Header(Triplanar)]
        _TriplanarScale ("Tri-planar Scale", Float) = 1
        _TriplanarBlendSharpness ("Tri-planar Blend Sharpness", Float) = 1
    }
    SubShader
    {
        Pass
        {
            // The value of the LightMode Pass tag must match the ShaderTagId in ScriptableRenderContext.DrawRenderers
            Tags
            {
                "LightMode" ="Lit"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "SpectralCore.hlsl"
            #include "SpectralTransforms.hlsl"
            #include "Lighting.hlsl"
            #include "Triplanar.hlsl"

            CBUFFER_START(UnityPerMaterial)
                real4 _PeakColor;
                real4 _HighlandsColor;
                real4 _MidlandsColor;
                real4 _LowlandsColor;
                real4 _WaterBedColor;
                real _NormalStrength;
                real _TriplanarBlendSharpness;
                real _TriplanarScale;
                real _Smoothness;
                real _SlopeMultiplier;
                real _WaterLevel;
                real _LowToMid;
                real _MidToHigh;
                real _HighToPeak;
                real _MaxAltitude;
                sampler2D _Normal;
                sampler2D _SmoothnessTex;
            CBUFFER_END

            struct Attributes
            {
                real4 posOS : POSITION;
                real2 uv : TEXCOORD0;
                real3 normOS : NORMAL;
                real4 tanOS : TANGENT;
            };

            struct Varyings
            {
                real4 posCS : SV_POSITION;
                real2 uv : TEXCOORD0;
                real3 normWS : NORMAL;
                real4 tanWS : TEXCOORD1;
                real3 posWS : TEXCOORD2;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.posCS = TransformObjectToHClip(IN.posOS);
                OUT.posWS = TransformObjectToWorld(IN.posOS);
                OUT.normWS = TransformObjectToWorldNormal(IN.normOS);
                OUT.tanWS.xyz = TransformObjectToWorldDir(IN.tanOS.xyz);
                OUT.tanWS.w = IN.tanOS.w;
                OUT.uv = IN.uv;
                return OUT;
            }

            real4 BlendElevation(real height, real slope, out real waterMask)
            {
                real remappedHeight = Remap(0, _MaxAltitude, 0, 1, height);
                real t1 = smoothstep(_WaterLevel, _LowToMid, remappedHeight);
                real t2 = smoothstep(_LowToMid, _MidToHigh, remappedHeight);
                real t3 = smoothstep(_MidToHigh, _HighToPeak, remappedHeight);
                real t4 = smoothstep(_HighToPeak, 1.0, remappedHeight);
                real4 waterLowBlend = lerp(_WaterBedColor, _LowlandsColor, t1);
                real4 lowMidBlend = lerp(waterLowBlend, _MidlandsColor, t2);
                real4 midHighBlend = lerp(lowMidBlend, _HighlandsColor, t3);
                real4 finalColor = lerp(midHighBlend, _PeakColor, t4);
                finalColor = lerp(finalColor, _HighlandsColor, 1-pow(slope,_SlopeMultiplier));
                waterMask = 1 - t1;
                return finalColor;
            }

   
            real4 frag(Varyings IN) : SV_TARGET
            {

                // Normalize the interpolated vectors
                real3 normWS = normalize(IN.normWS);
                real3 tanWS = normalize(IN.tanWS.xyz);
                real3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.posWS.xyz);
                real3 ambient = SampleSHSimple(normWS);
                real3 slope = dot(normWS, normalize(IN.posWS));
                real waterMask;
                real4 albedo = BlendElevation(IN.uv.x, slope, waterMask);
                real4 smoothness = tex2D(_SmoothnessTex, IN.uv) * _Smoothness * waterMask;;
                albedo.xyz *= ambient.xyz;
                // Sample and unpack the normal map (gives a tangent-space normal)
                real3 normTS = TriplanarNormalMap(IN.posWS, normWS,_Normal,_NormalStrength,_TriplanarScale,_TriplanarBlendSharpness,0);
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
                    albedo += LightContribution(info,1);
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
                    albedo += LightContribution(info,1);
                }
                return albedo;
            }
            ENDHLSL
        }
        Pass
        {
            Tags
            {
                "LightMode" = "DepthOnly"
            }
            ZWrite On
            HLSLPROGRAM
            #include "SpectralDepth.hlsl"
            #pragma vertex DepthOnlyVert
            #pragma fragment DepthOnlyFrag
            ENDHLSL
        }

    }
}