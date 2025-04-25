using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Spectral.Rendering
{
    [CreateAssetMenu(menuName = "Rendering/SpectrumPipelineAsset")]
    public class SpectralRenderPipelineAsset : RenderPipelineAsset<SpectralRenderPipelineInstance>
    {
        public int depthBufferBits = 24;
        public ShadowResolution shadowResolution;

        protected override RenderPipeline CreatePipeline()
        {
            GraphicsSettings.useScriptableRenderPipelineBatching = true;
            return new SpectralRenderPipelineInstance(this);
        }
    }
}