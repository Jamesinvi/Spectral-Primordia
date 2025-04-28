#include "SpectralCore.hlsl"

#define MAXLIGHTCOUNT 16
CBUFFER_START(Lights)
    real4 _LightColors[MAXLIGHTCOUNT];
    real4 _LightData[MAXLIGHTCOUNT];
    real4 _LightSpotDirs[MAXLIGHTCOUNT];
CBUFFER_END


real3 DiffuseDirectionalLight(real3 albedo, real3 lightCol, real3 lightDirWS, real3 surfNormWS, out real NdotL)
{
    NdotL = max(dot(surfNormWS, lightDirWS), 0.0);
    return albedo * lightCol * NdotL;
}

real3 DiffusePointLight(real3 normWs, real3 albedo, real3 fragPosWS, real3 lightPosWS, real range, real3 color, out real NdotL)
{
    real3 lightVec = lightPosWS - fragPosWS;
    NdotL = max(dot(lightVec, normWs), 0);
    real d = length(lightVec);
    real h = smoothstep(1, 0, (d * d) / (range * range));
    float attenuation = saturate(h);
    return albedo * NdotL * color * attenuation;
}

real3 SampleSHSimple(real3 normWS)
{
    real4 SHCoefficients[7];
    SHCoefficients[0] = unity_SHAr;
    SHCoefficients[1] = unity_SHAg;
    SHCoefficients[2] = unity_SHAb;
    SHCoefficients[3] = unity_SHBr;
    SHCoefficients[4] = unity_SHBg;
    SHCoefficients[5] = unity_SHBb;
    SHCoefficients[6] = unity_SHC;
    return SampleSH9(SHCoefficients, normWS);
}


real Fresnel(real3 normWS, real3 viewDir, real power)
{
    return pow((1.0 - saturate(dot(normalize(normWS), normalize(viewDir)))), power);
}


real3 GetSpecular(real3 normWS, real3 lightDirWS, real3 lightColor, real smoothness, real specularPower, real3 viewDir)
{
    // Blinn-Phong
    real3 halfAngle = normalize(lightDirWS + viewDir);
    real specAngle = saturate(dot(normWS, halfAngle));
    real n = lerp(1.0, 128.0, smoothness);

    // Normalization so energy roughly sums to 1
    real normalization = (n + 2.0) * (0.5 / PI);
    real rawSpec = normalization * pow(specAngle, n);

    // Scale by smoothness to drop amplitude on rough surfaces
    rawSpec *= smoothness;

    // Schlick‑style Fresnel
    real fresnelVal = Fresnel(normWS, viewDir, specularPower);
    real3 Fschlick = lerp(lightColor * 0.04, lightColor, fresnelVal);

    return rawSpec * Fschlick;
}


struct ShadingInfo
{
    real3 posWS;
    real3 normWS;
    real3 viewDir;
    real4 albedo;
    real smoothness;
    int lightType;
    real4 lightData;
    real3 lightCol;
    real3 reflectionCol;
};

real4 LightContribution(ShadingInfo surf, real contributionMultiplier)
{
    real4 contribution = real4(0, 0, 0, 0);
    if (surf.lightType == -1)
    {
        real NdotL;
        real3 diffuse = DiffuseDirectionalLight(surf.albedo.rgb, surf.lightCol, surf.lightData.xyz, surf.normWS, NdotL);
        real3 specular = GetSpecular(surf.normWS, surf.lightData, surf.lightCol * surf.reflectionCol, surf.smoothness, 1, surf.viewDir) * NdotL;
        contribution+= float4((diffuse + specular) * contributionMultiplier,0);
    }
    if (surf.lightType == -2)
    {
        real NdotL;
        real3 diffuse = DiffusePointLight(surf.normWS, surf.albedo.rgb, surf.posWS, surf.lightData, surf.lightData.w, surf.lightCol, NdotL);
        real3 specular =  GetSpecular(surf.normWS, surf.lightData, surf.lightCol * surf.reflectionCol, surf.smoothness, 1, surf.viewDir) * NdotL;
        contribution+= float4((diffuse + specular) * contributionMultiplier,0);
    }
    return contribution;
}
