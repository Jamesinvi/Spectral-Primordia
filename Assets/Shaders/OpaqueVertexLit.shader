Shader
"Spectral/OpaqueVertexLit"
{
    Properties
    {
        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _MainTex("Main Texture", 2D) = "white"{}
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
            #include "SpectralCore.hlsl"
            #include "SpectralTransforms.hlsl"
            #include "Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                sampler2D _MainTex;
                float4 _MainTex_ST;
            CBUFFER_END

            #define MAXLIGHTCOUNT 16

            CBUFFER_START(Lights)
                float4 _LightColors[MAXLIGHTCOUNT];
                float4 _LightData[MAXLIGHTCOUNT];
                float4 _LightSpotDirs[MAXLIGHTCOUNT];
            CBUFFER_END

            struct Attributes
            {
                float4 posOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normOS : NORMAL;
            };

            struct Varyings
            {
                float4 posCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normWS : NORMAL;
                float4 vertexLight : TEXCOORD1;
            };


            float4 LightContribution(in float3 positionWS, in float3 normalWS, int i, inout float4 albedo)
            {
                float NdotL;
                if (_LightColors[i].w == -1)
                {
                    albedo.xyz += DiffuseDirectionalLight(albedo.rgb, _LightColors[i].rgb, _LightData[i].xyz,normalWS, NdotL);
                }
                if (_LightColors[i].w == -2)
                {
                    albedo.xyz += DiffusePointLight(normalWS, albedo.rgb, positionWS, _LightData[i].xyz,_LightData[i].w, _LightColors[i].rgb, NdotL);
                }
                return albedo;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 posWS = TransformObjectToWorld(IN.posOS);

                OUT.posCS = TransformObjectToHClip(IN.posOS);
                OUT.normWS = TransformObjectToWorldNormal(IN.normOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                OUT.vertexLight = float4(1, 1, 1, 1);

                for (int id = 0; id < min(unity_LightData.y, 4); id++)
                {
                    int i = unity_LightIndices[0][id];
                    OUT.vertexLight = LightContribution(posWS, OUT.normWS, i, OUT.vertexLight);
                }

                for (int id2 = 4; id2 < min(unity_LightData.y, 8); id2++)
                {
                    int i = unity_LightIndices[1][id2 - 4];
                    OUT.vertexLight = LightContribution(posWS, OUT.normWS, i, OUT.vertexLight);
                }


                return OUT;
            }


            float4 frag(Varyings IN) : SV_TARGET
            {
                float4 ambient = float4(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w, 1);
                float4 albedo = tex2D(_MainTex, IN.uv) * _Color * ambient * IN.vertexLight;
                return albedo;
            }
            ENDHLSL
        }
    }
}