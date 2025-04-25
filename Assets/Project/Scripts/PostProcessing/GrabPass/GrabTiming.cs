using UnityEngine.Rendering.Universal;

namespace URPGrabPass
{
    internal enum GrabTiming
    {
        AfterOpaques,
        AfterTransparents,
        BeforePostProcessing
    }

    internal static class GrabTimingExtensions
    {
        internal static RenderPassEvent ToRenderPassEvent(this GrabTiming grabTiming)
        {
            return grabTiming switch {
                GrabTiming.AfterOpaques => RenderPassEvent.AfterRenderingSkybox,
                GrabTiming.AfterTransparents => RenderPassEvent.AfterRenderingTransparents,
                GrabTiming.BeforePostProcessing => RenderPassEvent.BeforeRenderingPostProcessing - 10,
                _ => throw new System.ArgumentOutOfRangeException(nameof(grabTiming), grabTiming, null)
            };;
        }
    }
}