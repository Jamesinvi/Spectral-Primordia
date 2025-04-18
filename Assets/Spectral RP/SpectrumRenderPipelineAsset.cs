using UnityEngine;
using UnityEngine.Rendering;

namespace Spectral_RP
{
    [CreateAssetMenu(menuName = "Rendering/SpectrumPipelineAsset")]
    public class SpectrumRenderPipelineAsset : RenderPipelineAsset<SpectrumRenderPipelineInstance>
    {
        public int depthBufferBits = 24;
        public ShadowResolution shadowResolution;
        protected override RenderPipeline CreatePipeline()
        {
            GraphicsSettings.useScriptableRenderPipelineBatching = true;
            return new SpectrumRenderPipelineInstance(this);
        }
    }
}