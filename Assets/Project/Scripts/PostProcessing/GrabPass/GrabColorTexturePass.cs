using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URPGrabPass
{
    internal class GrabColorTexturePass : ScriptableRenderPass, System.IDisposable
    {
        const string renderTag = nameof(GrabColorTexturePass);
        private readonly ProfilingSampler profilSampler = new ProfilingSampler(renderTag);

        private readonly int grabbTexture;
        private RTHandle grabbHandle;

        internal GrabColorTexturePass(GrabTiming timing, string grabbTextureName)
        {
            renderPassEvent = timing.ToRenderPassEvent();
            grabbTexture = Shader.PropertyToID(grabbTextureName);
            grabbHandle = RTHandles.Alloc(grabbTexture, grabbTextureName);
        }

        ~GrabColorTexturePass() => Dispose();
        public void Dispose()
        {
            if (grabbHandle == null) return;
            grabbHandle.Release();
            grabbHandle = null;
        }



        public override void FrameCleanup(CommandBuffer cmd) => cmd.ReleaseTemporaryRT(grabbTexture);
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (grabbHandle == null) return;
            cmd.GetTemporaryRT(grabbTexture, cameraTextureDescriptor);
            cmd.SetGlobalTexture(grabbTexture, grabbHandle);
        }



        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Preview) return;
            CommandBuffer cmd = CommandBufferPool.Get(renderTag);
            using (new ProfilingScope(cmd, profilSampler)) Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        private void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (grabbHandle == null) return;
            ScriptableRenderer renderer = renderingData.cameraData.renderer;
            Blit(cmd, renderer.cameraColorTargetHandle, grabbHandle);
        }
    }
}