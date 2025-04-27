Shader"Spectral/Debug/Depth"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white"{}
        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
    }
    SubShader
    {
        // The value of the LightMode Pass tag must match the ShaderTagId in ScriptableRenderContext.DrawRenderers
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        Cull Off Lighting Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Tags
            {
                "LightMode" = "Unlit"
            }
            ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "../SpectralCore.hlsl"
            #include "../SpectralTransforms.hlsl"


            sampler2D _CameraDepth;
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
            CBUFFER_END


            struct Attributes
            {
                float4 posOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 posCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.posCS = TransformObjectToHClip(IN.posOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_TARGET
            {
                float rawDepth = tex2D(_CameraDepth, IN.uv).r; 
                float linearDepth01 = Linear01Depth(rawDepth, _ZBufferParams);
                return float4(linearDepth01, linearDepth01, linearDepth01, 1.0);
            }
            ENDHLSL
        }
    }
}