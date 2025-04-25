using UnityEngine;

namespace Spectral.Rendering
{
    public static class LightShaderData
    {
        public static readonly int LightColorID = Shader.PropertyToID("_LightColors");
        public static readonly int LightDataID = Shader.PropertyToID("_LightData");
        public static readonly int LightSpotDirID = Shader.PropertyToID("_LightSpotDirs");
    }
}