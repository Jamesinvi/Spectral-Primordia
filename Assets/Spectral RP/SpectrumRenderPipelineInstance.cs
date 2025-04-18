using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using LightType = UnityEngine.LightType;
using RenderSettings = UnityEngine.RenderSettings;

namespace Spectral_RP
{
    public class SpectrumRenderPipelineInstance : RenderPipeline
    {
        private const int MaxLightCount = 16;

        private readonly Vector4[] _lightColor = new Vector4[MaxLightCount];
        private readonly Vector4[] _lightData = new Vector4[MaxLightCount];
        private readonly Vector4[] _lightSpotDir = new Vector4[MaxLightCount];
        private readonly SpectrumRenderPipelineAsset _renderPipelineAsset;
        private readonly int _shadowResolution;
        private readonly int _depthBufferBits;

        public SpectrumRenderPipelineInstance(SpectrumRenderPipelineAsset asset)
        {
            _renderPipelineAsset = asset;
            _depthBufferBits = asset.depthBufferBits;
            _shadowResolution = ((int)asset.shadowResolution + 1) * 512;
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            throw new NotImplementedException();
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            BeginContextRendering(context, cameras);
            // Create and schedule a command to clear the current render target
            // Iterate over all Cameras
            foreach (Camera camera in cameras)
            {
                BeginCameraRendering(context, camera);

                // Get the culling parameters from the current Camera
                camera.TryGetCullingParameters(out ScriptableCullingParameters cullingParameters);
                // Use the culling parameters to perform a cull operation, and store the results
                CullingResults cullingResults = context.Cull(ref cullingParameters);
                // Update the value of built-in shader variables, based on the current Camera
                context.SetupCameraProperties(camera);
                SetupLights(camera, context, ref cullingResults);

                //Get the setting from camera component
                bool drawSkyBox = camera.clearFlags == CameraClearFlags.Skybox;
                bool clearDepth = camera.clearFlags != CameraClearFlags.Nothing;
                bool clearColor = camera.clearFlags == CameraClearFlags.Color;

                CommandBuffer cmd = CommandBufferPool.Get("Clear");
                cmd.ClearRenderTarget(clearDepth, clearColor, camera.backgroundColor);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);


                // Schedule a command to draw the Skybox if required
                if (drawSkyBox && RenderSettings.skybox != null) RenderSkybox(context, camera);

                // Setup
                // Tell Unity which geometry to draw, based on its LightMode Pass tag value
                ShaderTagId unlitTagId = ShaderTags.UnlitShaderTag;
                ShaderTagId litTagId = ShaderTags.LitShaderTag;
                ShaderTagId shadowTagID = ShaderTags.ShadowShaderTag;
                // Tell Unity how to sort the geometry, based on the current Camera
                SortingSettings sortingSettings = new(camera);
                

                // Tell Unity how to filter the culling results, to further specify which geometry to draw
                DrawingSettings unlitDrawingSettings = new(unlitTagId, sortingSettings);
                DrawingSettings litDrawingSettings = new(litTagId, sortingSettings)
                {
                    perObjectData = PerObjectData.LightIndices | PerObjectData.LightData
                };
                // Use these default settings for drawing stock shaders
                DrawingSettings defaultDrawingSettings = new(ShaderTags.PassNameDefault, sortingSettings);
                
                defaultDrawingSettings.SetShaderPassName(1, ShaderTags.PassNameDefault);
                // Use FilteringSettings.defaultValue to specify no filtering
                FilteringSettings filteringSettings = new(RenderQueueRange.all);

                // Opaque
                sortingSettings.criteria = SortingCriteria.CommonOpaque;
                unlitDrawingSettings.sortingSettings = sortingSettings;
                filteringSettings.renderQueueRange = RenderQueueRange.opaque;
                RenderObjects("Unlit Opaques", context, cullingResults, filteringSettings, unlitDrawingSettings);

                // Opaque
                sortingSettings.criteria = SortingCriteria.CommonOpaque;
                litDrawingSettings.sortingSettings = sortingSettings;
                filteringSettings.renderQueueRange = RenderQueueRange.opaque;
                RenderObjects("Lit Opaques", context, cullingResults, filteringSettings, litDrawingSettings);

                //Opaque default
                defaultDrawingSettings.sortingSettings = sortingSettings;
                RenderObjects("Render Opaque Objects Default Pass", context, cullingResults, filteringSettings, defaultDrawingSettings);

                
                // Transparent
                sortingSettings.criteria = SortingCriteria.CommonTransparent;
                unlitDrawingSettings.sortingSettings = sortingSettings;
                filteringSettings.renderQueueRange = RenderQueueRange.transparent;
                RenderObjects("Transparents", context, cullingResults, filteringSettings, unlitDrawingSettings);


                //Transparent default
                defaultDrawingSettings.sortingSettings = sortingSettings;
                RenderObjects("Render Transparent Objects Default Pass", context, cullingResults, filteringSettings, defaultDrawingSettings);


#if UNITY_EDITOR
                if (Handles.ShouldRenderGizmos())
                {
                    context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
                    context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
                }
#endif


                context.Submit();
                EndCameraRendering(context, camera);
            }

            // Instruct the graphics API to perform all scheduled commands
            EndContextRendering(context, cameras);
        }

        public void SetupLights(Camera cam, ScriptableRenderContext context, ref CullingResults cullResults)
        {
            for (var i = 0; i < MaxLightCount; i++)
            {
                _lightColor[i] = Vector4.zero;
                _lightData[i] = Vector4.zero;
                _lightSpotDir[i] = Vector4.zero;

                if (i >= cullResults.visibleLights.Length) continue;
                VisibleLight visibleLight = cullResults.visibleLights[i];

                if (visibleLight.lightType == LightType.Directional)
                {
                    _lightData[i] = visibleLight.localToWorldMatrix.MultiplyVector(Vector3.back);
                    _lightColor[i] = visibleLight.finalColor;
                    _lightColor[i].w = -1; //for identifying it is a directional light in shader
                }
                else if (visibleLight.lightType == LightType.Point)
                {
                    _lightData[i] = visibleLight.localToWorldMatrix.GetPosition();
                    _lightData[i].w = visibleLight.range;
                    _lightColor[i] = visibleLight.finalColor;
                    _lightColor[i].w = -2; //for identifying it is a point light in shader
                }
                else if (visibleLight.lightType == LightType.Spot)
                {
                    _lightData[i] = visibleLight.localToWorldMatrix.GetColumn(3);
                    _lightData[i].w = 1f / Mathf.Max(visibleLight.range * visibleLight.range, 0.00001f);

                    _lightSpotDir[i] = visibleLight.localToWorldMatrix.GetColumn(2);
                    _lightSpotDir[i].x = -_lightSpotDir[i].x;
                    _lightSpotDir[i].y = -_lightSpotDir[i].y;
                    _lightSpotDir[i].z = -_lightSpotDir[i].z;
                    _lightColor[i] = visibleLight.finalColor;

                    float outerRad = Mathf.Deg2Rad * 0.5f * visibleLight.spotAngle;
                    float outerCos = Mathf.Cos(outerRad);
                    float outerTan = Mathf.Tan(outerRad);
                    float innerCos = Mathf.Cos(Mathf.Atan(46f / 64f * outerTan));
                    float angleRange = Mathf.Max(innerCos - outerCos, 0.001f);

                    //Spotlight attenuation
                    _lightSpotDir[i].w = 1f / angleRange;
                    _lightColor[i].w = -outerCos * _lightSpotDir[i].w;
                }
                // If it's not a point / directional / spot light, we ignore the light.
            }

            CommandBuffer cmdLight = CommandBufferPool.Get("Set-up Light Buffer");
            cmdLight.SetGlobalVectorArray(LightShaderData.LightDataID, _lightData);
            cmdLight.SetGlobalVectorArray(LightShaderData.LightColorID, _lightColor);
            cmdLight.SetGlobalVectorArray(LightShaderData.LightSpotDirID, _lightSpotDir);
            context.ExecuteCommandBuffer(cmdLight);
            CommandBufferPool.Release(cmdLight);
        }

        public static void RenderSkybox(ScriptableRenderContext context, Camera camera)
        {
            RendererList renderList = context.CreateSkyboxRendererList(camera);
            CommandBuffer commandBuffer = CommandBufferPool.Get("Render Skybox");
            commandBuffer.DrawRendererList(renderList);
            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        public static void RenderObjects(string name, ScriptableRenderContext context, CullingResults cull,
            FilteringSettings filterSettings, DrawingSettings drawSettings)
        {
            RendererListParams listParams = new(cull, drawSettings, filterSettings);
            RendererList renderList = context.CreateRendererList(ref listParams);
            CommandBuffer commandBuffer = CommandBufferPool.Get(name);
            commandBuffer.name = name;
            commandBuffer.DrawRendererList(renderList);
            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }
    }
}