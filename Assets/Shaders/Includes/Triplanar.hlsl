#include "SpectralCore.hlsl"

real3 TriplanarNormalMap(real3 posWS, real3 normWS, sampler2D tex, real normalStrength, real triplanarScale, real blendSharpness, real scrollSpeed, real strengthNoise)
{
    real3 scrolledPos = real3(posWS.x + scrollSpeed, posWS.y - scrollSpeed * .5, posWS.z + scrollSpeed * .15);
    real3 triUVA = (scrolledPos) * triplanarScale;
    float3 nAx = UnpackNormalScale(tex2D(tex, triUVA.zy), normalStrength * strengthNoise);
    float3 nAy = UnpackNormalScale(tex2D(tex, triUVA.xz), normalStrength * strengthNoise);
    float3 nAz = UnpackNormalScale(tex2D(tex, triUVA.xy), normalStrength * strengthNoise);
    real3 blendWeight = pow(abs(normWS), blendSharpness);
    real wsum = blendWeight.x + blendWeight.y + blendWeight.z;
    blendWeight = blendWeight / wsum;
    real3 normTS = nAx * blendWeight.x + nAy * blendWeight.y + nAz * blendWeight.z;
    return normTS;
}

real3 TriplanarTextureMap(real3 posWS, real3 normWS, sampler2D tex, real triplanarScale, real blendSharpness, real scrollSpeed)
{
    real3 scrolledPos = real3(posWS.x + scrollSpeed, posWS.y - scrollSpeed * .5, posWS.z + scrollSpeed * .15);
    real3 triUVA = (scrolledPos) * triplanarScale;
    float3 nAx = tex2D(tex, triUVA.zy);
    float3 nAz = tex2D(tex, triUVA.xy);
    float3 nAy = tex2D(tex, triUVA.xz);
    real3 blendWeight = pow(abs(normWS), blendSharpness);
    real wsum = blendWeight.x + blendWeight.y + blendWeight.z;
    blendWeight = blendWeight / wsum;
    real3 normTS = nAx * blendWeight.x + nAy * blendWeight.y + nAz * blendWeight.z;
    return normTS;
}
