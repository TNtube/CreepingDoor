using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URPGrabPass
{
    [System.Serializable]
    internal class GrabPassRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        private class GrabSettings
        {
            [Tooltip("When to grab color texture")]
            public GrabTiming timing = GrabTiming.AfterTransparents;
            [Tooltip("Texture name to use in the shader")]
            public string grabbedTextureName = "_CameraColorTexture";
        }

        [System.Serializable]
        private class RenderSettings
        {
            [Tooltip("Do Enable Rendering Objects")]
            public bool enableRendering = true;
            [Tooltip("How to sort objects during rendering")]
            public SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
            [Tooltip("Light Mode of shaders that use grabbed texture")]
            public string[] shaderLightModes = new string[] { "UseColorTexture" };
        }



        [SerializeField]
        private GrabSettings grabSettings = new GrabSettings();
        [SerializeField]
        private RenderSettings renderSettings = new RenderSettings();

        private GrabColorTexturePass grabColorPass;
        private UseColorTexturePass useColorPass;



        public override void Create()
        {
            grabColorPass = new GrabColorTexturePass(grabSettings.timing, grabSettings.grabbedTextureName);
            if (renderSettings.enableRendering) useColorPass = new UseColorTexturePass(grabSettings.timing, renderSettings.shaderLightModes, renderSettings.sortingCriteria);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Preview) return;
            grabColorPass.ConfigureInput(ScriptableRenderPassInput.Color);
            if (renderSettings.enableRendering) useColorPass.ConfigureInput(ScriptableRenderPassInput.Color);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Preview) return;
            renderer.EnqueuePass(grabColorPass);
            if (renderSettings.enableRendering) renderer.EnqueuePass(useColorPass);
        }

        protected override void Dispose(bool disposing)
        {
            grabColorPass.Dispose();
        }
    }
}