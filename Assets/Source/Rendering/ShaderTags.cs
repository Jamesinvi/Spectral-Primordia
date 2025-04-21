using UnityEngine.Rendering;

namespace Spectral.Rendering
{
    public static class ShaderTags
    {
        public static readonly ShaderTagId UnlitShaderTag = new("Unlit");
        public static readonly ShaderTagId LitShaderTag = new("Lit");
        public static readonly ShaderTagId DepthShaderTag = new("DepthOnly");
        public static readonly ShaderTagId PassNameDefault = new("SRPDefaultUnlit"); //The shader pass tag for replacing shaders without pass
        public static readonly ShaderTagId ShadowShaderTag = new("ShadowCaster"); //For shadow
    }
}