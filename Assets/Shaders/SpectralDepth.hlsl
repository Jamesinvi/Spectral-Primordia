#include "SpectralTransforms.hlsl"

struct DepthOnlyAttributes
{
    float4 posOS : POSITION;
    float2 texCoord : TEXCOORD0;
};

struct DepthOnlyVaryings
{
    float4 posCS : SV_POSITION;
};

DepthOnlyVaryings DepthOnlyVert(DepthOnlyAttributes v)
{
    DepthOnlyVaryings o;

    o.posCS = TransformObjectToHClip(v.posOS.xyz);
    return o;
}

float4 DepthOnlyFrag(DepthOnlyVaryings i) : SV_Target
{
    return 1;
}
