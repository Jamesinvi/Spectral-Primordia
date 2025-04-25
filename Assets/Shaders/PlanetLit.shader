Shader
"Spectral/PlanetLit"
{
    Properties
    {
        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _MinTess("Min Tess Factor", Range(1,64)) = 1
        _MaxTess("Max Tess Factor", Range(1,64)) = 16
        _MinDistance("Tess Start Dist", Float) = 5
        _MaxDistance("Tess End Dist", Float) = 50
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


            CBUFFER_START(UnityPerMaterial)
                real4 _Color;
            CBUFFER_END

            #define MAXLIGHTCOUNT 16

            CBUFFER_START(Lights)
                real4 _LightColors[MAXLIGHTCOUNT];
                real4 _LightData[MAXLIGHTCOUNT];
                real4 _LightSpotDirs[MAXLIGHTCOUNT];
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

            real4 frag(Varyings IN) : SV_TARGET
            {
                // Normalize the interpolated vectors
                real3 normWS = normalize(IN.normWS);
                real3 tanWS = normalize(IN.tanWS.xyz);
                real3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.posWS.xyz);

                real3 ambient = SampleSHSimple(normWS);
                real4 albedo = _Color;
                real4 smoothness = 0.2;
                albedo.xyz *= ambient.xyz;
                real3 normTS = float3(0.0, 0.0, 1.0);
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
                    albedo += LightContribution(info);
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
                    albedo += LightContribution(info);
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
            ColorMask R
            HLSLPROGRAM
            #include "SpectralDepth.hlsl"
            #pragma vertex DepthOnlyVert
            #pragma fragment DepthOnlyFrag
            ENDHLSL
        }

    }
}