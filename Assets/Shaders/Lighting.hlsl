float3 DiffuseDirectionalLight(float3 albedo, float3 lightCol, float3 lightDirWS, float3 surfNormWS, out float NdotL)
{
    NdotL = max(dot(surfNormWS, lightDirWS), 0.0);
    return albedo * lightCol * NdotL;
}

float3 DiffusePointLight(float3 normWs, float3 albedo, float3 fragPosWS, float3 lightPosWS, float range, float3 color, out float NdotL)
{
    float3 lightVec = lightPosWS - fragPosWS;
    float d = length(lightVec);
    lightVec = normalize(lightVec);
    float attenuation = saturate(1.0 - (d * d) / (range * range)); // Smooth quadratic
    NdotL = max(dot(lightVec, normWs), 0);
    return albedo * NdotL * color * attenuation;
}

float3 SampleSHSimple(float3 normWS)
{
    float4 SHCoefficients[7];
    SHCoefficients[0] = unity_SHAr;
    SHCoefficients[1] = unity_SHAg;
    SHCoefficients[2] = unity_SHAb;
    SHCoefficients[3] = unity_SHBr;
    SHCoefficients[4] = unity_SHBg;
    SHCoefficients[5] = unity_SHBb;
    SHCoefficients[6] = unity_SHC;
    return SampleSH9(SHCoefficients, normWS);
}


float Fresnel(float3 normWS, float3 viewDir, float power)
{
    return pow((1.0 - saturate(dot(normalize(normWS), normalize(viewDir)))), power);
}


float3 GetSpecular(
    float3 normWS,
    float3 lightDirWS,
    float3 lightColor,
    float smoothness,
    float specularPower,
    float3 viewDir)
{
    // Blinn-Phong
    float3 halfAngle = normalize(lightDirWS + viewDir);
    float specAngle = saturate(dot(halfAngle, normWS));
    float spec = pow(specAngle, smoothness);
    float fresnel = Fresnel(normWS, viewDir, specularPower);
    float reflectance=lerp(0.04, 1, fresnel);
    return normalize(lightColor) * spec * reflectance;
}
