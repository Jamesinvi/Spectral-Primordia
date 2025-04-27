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
    }
}