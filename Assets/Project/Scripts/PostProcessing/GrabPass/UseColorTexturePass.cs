using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URPGrabPass
{
    internal class UseColorTexturePass : ScriptableRenderPass
    {
        const string renderTag = nameof(UseColorTexturePass);
        private readonly ProfilingSampler profilSampler = new ProfilingSampler(renderTag);

        private readonly List<ShaderTagId> shaderTagIds = new List<ShaderTagId>();
        private readonly SortingCriteria sortingCriteria;
        private FilteringSettings filteringSettings;

        internal UseColorTexturePass(GrabTiming timing, string[] shaderLightModes, SortingCriteria sortingCriteria)
        {
            renderPassEvent = timing.ToRenderPassEvent() + 1;
            foreach (string shaderLightMode in shaderLightModes) shaderTagIds.Add(new ShaderTagId(shaderLightMode));
            this.filteringSettings = new FilteringSettings(RenderQueueRange.all);
            this.sortingCriteria = sortingCriteria;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilSampler)) Render(context, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        private void Render(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIds, ref renderingData, sortingCriteria);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }
    }
}