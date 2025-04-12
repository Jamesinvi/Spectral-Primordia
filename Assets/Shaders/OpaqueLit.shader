
Shader
"Spectral/OpaqueLit"
{
  Properties
    {
        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _MainTex("Main Texture", 2D) = "white"{}
        _Normal("Normal", 2D) = "bump" {}
        _NormalStrength("Normal Strength", Range(0,5)) = 1.0
        _Smoothness("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
      
        Pass
        {
            // The value of the LightMode Pass tag must match the ShaderTagId in ScriptableRenderContext.DrawRenderers
            Tags { "LightMode" ="Lit"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "SpectralCore.hlsl"
            #include "SpectralTransforms.hlsl"
            #include "Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float _NormalStrength;
            float _Smoothness;
			sampler2D _MainTex;
			sampler2D _Normal;
			float4 _MainTex_ST;
			float4 _Normal_ST;
            CBUFFER_END
		    #define MAXLIGHTCOUNT 16

			CBUFFER_START(Lights)
            float4 _LightColors[MAXLIGHTCOUNT];
            float4 _LightData[MAXLIGHTCOUNT];
            float4 _LightSpotDirs[MAXLIGHTCOUNT];
			CBUFFER_END
			
            struct Attributes
            {
                float4 posOS	: POSITION;
			    float2 uv       : TEXCOORD0;
                float3 normOS   : NORMAL;
				float4 tanOS	: TANGENT;
            };

            struct Varyings
            {
                float4 posCS	: SV_POSITION;
				float2 uv       : TEXCOORD0;
                float3 normWS   : NORMAL;
				float4 tanWS	: TEXCOORD1;
                float3 posWS	: TEXCOORD2;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                
                OUT.posCS = TransformObjectToHClip(IN.posOS);
            	OUT.posWS = TransformObjectToWorld(IN.posOS);
                OUT.normWS = TransformObjectToWorldNormal(IN.normOS);
				OUT.tanWS.xyz = TransformObjectToWorldDir(IN.tanOS.xyz);
				OUT.tanWS.w = IN.tanOS.w;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }



            float4 LightContribution(in float3 posWS, in float3 normWS, int i, inout float4 albedo)
            {
            	float adjSmoothness = exp2(((_Smoothness) * 10)+1);
            	float3 viewDir = normalize(mul(UNITY_MATRIX_I_V, float4(0,0,-1,0)).xyz);
                if(_LightColors[i].w == -1)
                {
                	float NdotL;
                	albedo.xyz += DiffuseDirectionalLight(albedo.rgb,_LightColors[i].rgb, _LightData[i].xyz, normWS, NdotL);
					albedo.xyz += GetSpecular(normWS,  _LightData[i].xyz, adjSmoothness, NdotL,viewDir);
				}
                if(_LightColors[i].w == -2){
                	float NdotL;
                    albedo.xyz += DiffusePointLight(normWS, albedo.rgb, posWS, _LightData[i].xyz, _LightData[i].w, _LightColors[i].rgb, NdotL);
					albedo.xyz += GetSpecular(normWS,  _LightData[i].xyz, adjSmoothness, NdotL,viewDir);

                }
                return albedo;
            }

            float4 frag (Varyings IN) : SV_TARGET
            {

                // Normalize the interpolated vectors
                float3 normWS = normalize(IN.normWS);
            	float3 tanWS = normalize(IN.tanWS.xyz);

				float3 ambient = SampleSHSimple(normWS);
                float4 albedo = tex2D(_MainTex,IN.uv) * _Color;
            	albedo.xyz*=ambient.xyz;
			    // Sample and unpack the normal map (gives a tangent-space normal)
			    float3 normTS = UnpackNormalScale(tex2D(_Normal, IN.uv),_NormalStrength);
				float3x3 TBN = CreateTangentToWorld(normWS, tanWS, IN.tanWS.w);// Tangent-bitangent-normal
                normWS = TransformTangentToWorld(normTS, TBN,true);
                for (int id = 0; id < min(unity_LightData.y,4); id++) 
				{
					int i = unity_LightIndices[0][id];
					albedo = LightContribution(IN.posWS, normWS, i,albedo);
				}

				for (int id2 = 4; id2 < min(unity_LightData.y,8); id2++) 
				{
					int i = unity_LightIndices[1][id2-4];
					albedo = LightContribution(IN.posWS, normWS,i,albedo);
				}
                return albedo;
                
            }
            ENDHLSL
        }
    }
}