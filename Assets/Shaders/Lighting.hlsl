real3 DiffuseDirectionalLight(real3 albedo, real3 lightCol, real3 lightDirWS, real3 surfNormWS, out real NdotL)
{
    NdotL = max(dot(surfNormWS, lightDirWS), 0.0);
    return albedo * lightCol * NdotL;
}

real3 DiffusePointLight(real3 normWs, real3 albedo, real3 fragPosWS, real3 lightPosWS, real range, real3 color, out real NdotL)
{
    real3 lightVec = lightPosWS - fragPosWS;
    real d = length(lightVec);
    lightVec = normalize(lightVec);
    real attenuation = saturate(1.0 - (d * d) / (range * range)); // Smooth quadratic
    NdotL = max(dot(lightVec, normWs), 0);
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
    real specAngle = saturate(dot(halfAngle, normWS));
    real spec = pow(specAngle, smoothness);
    real fresnel = Fresnel(normWS, viewDir, specularPower);
    real reflectance = lerp(0.04, 1, fresnel);
    return normalize(lightColor) * spec * reflectance;
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

real4 LightContribution(ShadingInfo surf)
{
    real adjSmoothness = exp2(((surf.smoothness) * 10) + 1);
    if (surf.lightType == -1)
    {
        real NdotL;
        surf.albedo.xyz += DiffuseDirectionalLight(surf.albedo.rgb, surf.lightCol, surf.lightData, surf.normWS, NdotL);
        surf.albedo.xyz += GetSpecular(surf.normWS, surf.lightData, surf.lightCol * surf.reflectionCol, adjSmoothness, 1, surf.viewDir) * NdotL;
    }
    if (surf.lightType == -2)
    {
        real NdotL;
        surf.albedo.xyz += DiffusePointLight(surf.normWS, surf.albedo.rgb, surf.posWS, surf.lightData, surf.lightData.w, surf.lightCol, NdotL);
        surf.albedo.xyz += GetSpecular(surf.normWS, surf.lightData, surf.lightCol * surf.reflectionCol, adjSmoothness, 1, surf.viewDir) * NdotL;
    }
    return surf.albedo;
}
