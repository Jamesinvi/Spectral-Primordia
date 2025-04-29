Shader
"Spectral/OpaqueVertexLit"
{
    Properties
    {
        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _MainTex("Main Texture", 2D) = "white"{}
        _ToneTex("Tone Map", 2D) = "white"{}
        _Normal("Normal", 2D) = "bump" {}
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
            #include "../Includes/SpectralCore.hlsl"
            #include "../Includes/SpectralTransforms.hlsl"
            #include "../Includes/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                real4 _Color;
                sampler2D _MainTex;
                sampler2D _ToneTex;
                real4 _MainTex_ST;
            CBUFFER_END

            struct Attributes
            {
                real4 posOS : POSITION;
                real2 uv : TEXCOORD0;
                real3 normOS : NORMAL;
            };

            struct Varyings
            {
                real4 posCS : SV_POSITION;
                real2 uv : TEXCOORD0;
                real3 normWS : NORMAL;
                real4 vertexLight : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                real3 posWS = TransformObjectToWorld(IN.posOS);
                OUT.posCS = TransformObjectToHClip(IN.posOS);
                OUT.normWS = TransformObjectToWorldNormal(IN.normOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                real3 viewDir = normalize(_WorldSpaceCameraPos.xyz - posWS.xyz);
                real3 reflection = reflect(-viewDir, OUT.normWS); // Direction of ray after hitting the surface of object
                float lod = Remap(0, 1, 8, 0, 1);
                real3 reflectionCol = PLATFORM_SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflection, lod);

                real4 vertexLight = real4(1, 1, 1, 1);
                for (int id = 0; id < min(unity_LightData.y, 4); id++)
                {
                    int i = unity_LightIndices[0][id];
                    ShadingInfo info;
                    info.posWS = posWS;
                    info.normWS = OUT.normWS;
                    info.viewDir = viewDir;
                    info.smoothness = .5;
                    info.albedo = vertexLight;
                    info.lightType = _LightColors[i].w;
                    info.lightData = _LightData[i];
                    info.lightCol = _LightColors[i];
                    info.reflectionCol = reflectionCol;
                    vertexLight += LightContribution(info,1);
                }

                for (int id2 = 4; id2 < min(unity_LightData.y, 8); id2++)
                {
                    int i = unity_LightIndices[1][id2 - 4];
                    ShadingInfo info;
                    info.posWS = posWS;
                    info.normWS = OUT.normWS;
                    info.viewDir = viewDir;
                    info.albedo = vertexLight;
                    info.smoothness = .5;
                    info.lightType = _LightColors[i].w;
                    info.lightData = _LightData[i];
                    info.lightCol = _LightColors[i];
                    info.reflectionCol = reflectionCol;
                    vertexLight += LightContribution(info,1);
                }
                OUT.vertexLight = vertexLight;
                return OUT;
            }


            real4 frag(Varyings IN) : SV_TARGET
            {
                real4 ambient = real4(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w, 1);
                real4 tone = tex2D(_ToneTex, IN.uv);
                real4 albedo = tex2D(_MainTex, IN.uv) * _Color * ambient * IN.vertexLight;
                albedo * tone.g * tone.g; // AO
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
            #include "../Includes/SpectralDepth.hlsl"
            #pragma vertex DepthOnlyVert
            #pragma fragment DepthOnlyFrag
            ENDHLSL
        }
    }
}