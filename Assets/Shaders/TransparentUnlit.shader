
Shader"Spectral/TransparentUnlit"
{
  Properties
    {
        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _MainTex("Texture", 2D) = "white"{}
    }
    SubShader
    {
      
        // The value of the LightMode Pass tag must match the ShaderTagId in ScriptableRenderContext.DrawRenderers
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Cull Off Lighting Off ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode" = "Unlit" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "SpectralCore.hlsl"
            #include "SpectralTransforms.hlsl"

            CBUFFER_START (UnityPerMaterial)
            float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
            CBUFFER_END


            struct Attributes
            {
                float4 posOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 posCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.posCS = TransformObjectToHClip(IN.posOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag (Varyings IN) : SV_TARGET
            {
                float2 tiledUv = TRANSFORM_TEX(IN.uv, _MainTex);
				float4 texColor = tex2D(_MainTex, tiledUv);
                return texColor * _Color;
                
            }
            ENDHLSL
        }
    }
}