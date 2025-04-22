Shader "Spectral/Skybox"
{
    Properties
    {
        _HorizonColor("Horizon Color", Color) = (0,0,0,1)
        _ZenithColor("Zenith Color", Color) = (0,0,0,1)
        _GradientPower("Gradient", Float) = 1
        _StarColor("Star Color", Color) = (1,1,1,1)
        _StarPower("Star Power", Float) = 1
        _StarSize("Star Size", Float) = 1
        _StarHaloPower("Star Halo Power", Float) = 1
        _StarHaloStrength("Star Halo Strength", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #include "SpectralCore.hlsl"
            #include "SpectralTransforms.hlsl"
            #define MAXLIGHTCOUNT 16

            CBUFFER_START(Lights)
                real4 _LightData[MAXLIGHTCOUNT];
            CBUFFER_END

            real4 _HorizonColor;
            real4 _ZenithColor;
            real4 _StarColor;;
            real _GradientPower;
            real _StarPower;
            real _StarSize;
            real _StarHaloPower;
            real _StarHaloStrength;

            struct Attributes
            {
                real4 posOS : POSITION;
                real2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                real4 posCS : SV_POSITION;
                real2 uv : TEXCOORD0;
                real3 posWS : TEXCOORD1;
                real3 viewDir : TEXCOORD2;
            };


            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.posCS = TransformObjectToHClip(IN.posOS);
                real3 worldPos = TransformObjectToWorld(IN.posOS).xyz;
                OUT.viewDir = worldPos - _WorldSpaceCameraPos;
                OUT.uv = IN.uv;
                OUT.posWS = worldPos;
                return OUT;
            }

            real4 frag(Varyings IN) : SV_Target
            {
                real3 dir = normalize(IN.viewDir);
                real upAmt = saturate(dir.y);
                real downAmt = saturate(-dir.y);
                real3 col = _HorizonColor.rgb;
                col = lerp(col, _ZenithColor.rgb, pow(downAmt + upAmt, _GradientPower));

                real3 starDir = _LightData[0].xyz;
                float cosAngle = dot(dir, starDir);
                float cosAnglePow = SafePositivePow(saturate(cosAngle), _StarHaloPower);
                float cosThreshold = cos(radians(_StarSize));
                float edgeWidth = 0.01;
                float brightness = smoothstep(cosThreshold - edgeWidth, cosThreshold + edgeWidth, cosAngle);
                col += brightness * _StarColor.rgb * _StarPower;
                col += cosAnglePow * _StarColor * _StarHaloStrength;;
                // get integer pixel coords
                int2 pix = int2(IN.viewDir.xy);
                float noise = GenerateHashedRandomFloat(pix);
                float d = (noise - 0.5) * .001;
                col += d;
                return real4(col, 1);
            }
            ENDHLSL
        }
    }
}