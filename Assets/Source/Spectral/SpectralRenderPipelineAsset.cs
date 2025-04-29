using UnityEngine;
using UnityEngine.Rendering;

namespace Spectral
{
    [CreateAssetMenu(menuName = "Rendering/SpectrumPipelineAsset")]
    public class SpectralRenderPipelineAsset : RenderPipelineAsset<SpectralRenderPipelineInstance>
    {
        public ShadowResolution shadowResolution;

        protected override RenderPipeline CreatePipeline()
        {
            GraphicsSettings.useScriptableRenderPipelineBatching = true;
            return new SpectralRenderPipelineInstance(this);
        }

#if UNITY_EDITOR
        //==================== Default Materials =======================

        private const string MaterialDefaultsPath = "Assets/Materials/SpectralDefaultMaterials/";
        public override Material defaultMaterial => UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(MaterialDefaultsPath + "LitOpaque.mat");

        //==================== Default Shaders =======================

        public override Shader defaultShader => Shader.Find("Spectral/OpaqueLit");
    }
#endif
}
