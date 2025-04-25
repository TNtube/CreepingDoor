using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
[DisallowMultipleRendererFeature]
public class ShakeRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PassSettings
    {
        public float magnitude = 0.1f;
        public float frequency = 20f;
    }

    [SerializeField]
    private PassSettings shakeSettings = new PassSettings();
    private ShakeRenderPass shakePass;



    public override void Create()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
        shakePass = new ShakeRenderPass(shakeSettings);
    }



    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
        if (renderingData.cameraData.cameraType != CameraType.Game) return;
        shakePass.ConfigureInput(ScriptableRenderPassInput.Color);
    }



    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
        if (renderingData.cameraData.cameraType != CameraType.Game) return;
        renderer.EnqueuePass(shakePass);
    }



    protected override void Dispose(bool disposing)
    {
        if (shakePass == null) return;
        shakePass.Dispose();
    }
}