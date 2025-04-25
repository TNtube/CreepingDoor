using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using Project.Utils;

public class ShakeRenderPass : ScriptableRenderPass, System.IDisposable
{
    const string renderTag = "Custom Shake Pass";
    private readonly RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing - 1;

    const string shakeShader = "Hidden/ScreenShakeVR";
    const string shakeParameter = "_ShakeFac";
    const int shakePass = 0;

    private readonly ShakeRenderFeature.PassSettings settings;
    private readonly Material blitMaterial;

    private static ShakeRenderPass instance;
    private readonly List<ShakeEvent> activeShakes = new List<ShakeEvent>();
    private float shakeVal = 0f;




    private class ShakeEvent
    {
        private readonly float magnitude;
        private readonly float length;
        private float time;

        public bool Finished => time >= length;
        public float CurrentStrength => magnitude * Mathf.Clamp01(1f - time / length);

        public void Update(float deltaTime) => time += deltaTime;
        public ShakeEvent(float mag, float len)
        {
            magnitude = mag;
            length = len;
        }
    }




    
    public ShakeRenderPass(ShakeRenderFeature.PassSettings settings)
    {
        this.settings = settings;
        base.renderPassEvent = passEvent;
        base.requiresIntermediateTexture = true;
        base.profilingSampler = new ProfilingSampler(renderTag);
        blitMaterial = CoreUtils.CreateEngineMaterial(shakeShader);
        StaticUpdateHelper.StaticUpdate += Update;
        ShakeRenderPass.instance = this;
    }

    ~ShakeRenderPass() => Dispose();
    public void Dispose()
    {
        StaticUpdateHelper.StaticUpdate -= Update;
        CoreUtils.Destroy(blitMaterial);
    }



    #region RenderGraph Implementation

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        // Check Pass is needed
        if (Mathf.Approximately(shakeVal, 0f)) return;
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer) return;

        // Get Source Color Texture & Create Intermediaire/Target Texture
        TextureHandle source = resourceData.activeColorTexture;
        TextureDesc destinationDesc = renderGraph.GetTextureDesc(source);
        destinationDesc.name = $"CameraColor-{passName}";
        destinationDesc.clearBuffer = false;
        TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

        // Check everything good & Update shake settings
        if (!source.IsValid() || !destination.IsValid()) return;
        blitMaterial.SetFloat(shakeParameter, shakeVal);

        // Apply Blit Pass (Using Unity internal Utils Blit)
        // (WANING !! XR Support not implemented in 17.0.4. Founded it in 17.2.0 at least) (com.unity.render-pipelines.core)
        // Use Custom Implementation with PackageManager Git from https://github.com/LouisViel/com.unity.render-pipelines.core if needed
        RenderGraphUtils.BlitMaterialParameters parameters = new RenderGraphUtils.BlitMaterialParameters(source, destination, blitMaterial, shakePass);
        renderGraph.AddBlitPass(parameters, passName: passName);
        resourceData.cameraColor = destination;
    }

    /*class PassData
    {
        public TextureHandle color;
        public TextureHandle intermediate;
        public Material material;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (Mathf.Approximately(shakeVal, 0f)) return;
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer) return;

        // Set the texture size to match the camera target size
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        shakeTextureDescriptor.width = cameraData.cameraTargetDescriptor.width;
        shakeTextureDescriptor.height = cameraData.cameraTargetDescriptor.height;
        shakeTextureDescriptor.depthBufferBits = 0;

        TextureHandle srcCamColor = resourceData.activeColorTexture;
        TextureHandle intermediateTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, shakeTextureDescriptor, shakeTextureName, false);
        if (!srcCamColor.IsValid() || !intermediateTexture.IsValid()) return;
        blitMaterial.SetFloat(shakeParameter, shakeVal);

        using (IUnsafeRenderGraphBuilder builder = renderGraph.AddUnsafePass(passName, out PassData passData, profilingSampler)) {
            passData.color = srcCamColor;
            passData.intermediate = intermediateTexture;

            builder.SetRenderFunc(static (PassData passData, UnsafeGraphContext context) => ExecutePass(passData, context));
            builder.UseTexture(intermediateTexture, AccessFlags.Write);
            builder.UseTexture(srcCamColor, AccessFlags.Read);
        }

        using (IUnsafeRenderGraphBuilder builder = renderGraph.AddUnsafePass(passName, out PassData passData, profilingSampler)) {
            passData.color = intermediateTexture;
            passData.intermediate = srcCamColor;
            passData.material = blitMaterial;

            builder.SetRenderFunc(static (PassData passData, UnsafeGraphContext context) => ExecutePass(passData, context));
            builder.UseTexture(intermediateTexture, AccessFlags.Read);
            builder.UseTexture(srcCamColor, AccessFlags.Write);
        }
    }

    static void ExecutePass(PassData passData, UnsafeGraphContext context)
    {
        CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
        if (passData.material == null) Blitter.BlitCameraTexture(cmd, passData.color, passData.intermediate);
        else Blitter.BlitCameraTexture(cmd, passData.color, passData.intermediate, passData.material, shakePass);
        Debug.Log("Hey !!");
    }*/

    #endregion // RenderGraph Implementation



    #region Legacy Implementation

    [System.Obsolete]
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CameraData cameraData = renderingData.cameraData;
        if (cameraData.camera.cameraType != CameraType.Game) return;

        CommandBuffer cmd = CommandBufferPool.Get(passName);
        using (new ProfilingScope(cmd, profilingSampler)) Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    [System.Obsolete]
    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (blitMaterial == null) return;
        if (!Mathf.Approximately(shakeVal, 0f)) {
            blitMaterial.SetFloat(shakeParameter, shakeVal);
            Blit(cmd, ref renderingData, blitMaterial, shakePass);
        }
    }

    #endregion // Legacy Implementation




    void Update()
    {
        float shakeCumulation = 0f;
        for (int i = activeShakes.Count - 1; i >= 0; i--) {
            activeShakes[i].Update(Time.deltaTime);
            shakeCumulation += activeShakes[i].CurrentStrength;
            if (activeShakes[i].Finished) activeShakes.RemoveAt(i);
        }

        if (shakeCumulation > 0f) shakeVal = Mathf.PerlinNoise(Time.time * settings.frequency, 10.234896f) * shakeCumulation * settings.magnitude;
        else shakeVal = 0f;
    }




    public static void Shake(float magnitude, float length)
    {
        if (instance == null) return;
        instance.ShakeInternal(magnitude, length);
    }

    private void ShakeInternal(float magnitude, float length)
    {
        activeShakes.Add(new ShakeEvent(magnitude, length));
    }
}