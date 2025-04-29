Shader "Spectral/OpaqueUnlit"
{
    Properties
    {
        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _MainTex("Main Texture", 2D) = "white"{}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            // The value of the LightMode Pass tag must match the ShaderTagId in ScriptableRenderContext.DrawRenderers
            Tags
            {
                "LightMode" = "Unlit"
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "../Includes/SpectralCore.hlsl"
            #include "../Includes/SpectralTransforms.hlsl"

            CBUFFER_START(UnityPerMaterial)
                real4 _Color;
                sampler2D _MainTex;
                real4 _MainTex_ST;
            CBUFFER_END


            struct Attributes
            {
                real4 posOS : POSITION;
                real2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                real4 posCS : SV_POSITION;
                real2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.posCS = TransformObjectToHClip(IN.posOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            real4 frag(Varyings IN) : SV_TARGET
            {
                real2 tiledUv = TRANSFORM_TEX(IN.uv, _MainTex);
                real4 texColor = tex2D(_MainTex, tiledUv);
                return texColor * _Color;
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