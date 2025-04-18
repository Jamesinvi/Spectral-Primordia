#include "SpectralTransforms.hlsl"

struct DepthOnlyAttributes
{
    float4 posOS     : POSITION;
    float2 texCoord     : TEXCOORD0;
};

struct DepthOnlyVaryings
{
    float4 posCS   : SV_POSITION;
};

DepthOnlyVaryings DepthOnlyVert(DepthOnlyAttributes v)
{
    DepthOnlyVaryings o;

    o.posCS = TransformObjectToHClip(v.posOS.xyz);
    return o;
}

float4 DepthOnlyFrag(DepthOnlyVaryings i) : SV_Target
{
    float ndc = i.posCS.z / i.posCS.w;         // NDC z ∈ [–1,1]
    float d01 = ndc * 0.5 + 0.5;               // remap → [0,1]
    return float4(d01, d01, d01, 1);
}
