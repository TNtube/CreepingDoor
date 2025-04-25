using System.Runtime.CompilerServices;
using System;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
using Unity.Mathematics;

public static class ScreenShakeBlit
{
    class BlitPassData
    {
        public TextureHandle source;
        public TextureHandle destination;
        public Vector2 scale;
        public Vector2 offset;
        public int sourceSlice;
        public int destinationSlice;
        public int numSlices;
        public int sourceMip;
        public int destinationMip;
        public int numMips;
        public BlitFilterMode filterMode;
        public bool isXR;
    }

    public enum BlitFilterMode
    {
        /// <summary>
        /// Clamp to the nearest pixel when selecting which pixel to blit from.
        /// </summary>
        ClampNearest,
        /// <summary>
        /// Use bileanear filtering when selecting which pixels to blit from.
        /// </summary>
        ClampBilinear
    }

    public static void AddBlitPass(this RenderGraph graph,
            TextureHandle source,
            TextureHandle destination,
            Vector2 scale,
            Vector2 offset,
            int sourceSlice = 0,
            int destinationSlice = 0,
            int numSlices = -1,
            int sourceMip = 0,
            int destinationMip = 0,
            int numMips = 1,
            BlitFilterMode filterMode = BlitFilterMode.ClampBilinear,
            string passName = "Blit Pass Utility"
#if !CORE_PACKAGE_DOCTOOLS
                , [CallerFilePath] string file = "",
    [CallerLineNumber] int line = 0)
#endif
    {
        var sourceDesc = graph.GetTextureDesc(source);
        var destinationDesc = graph.GetTextureDesc(destination);
        int sourceMaxWidth = math.max(math.max(sourceDesc.width, sourceDesc.height), sourceDesc.slices);
        int sourceTotalMipChainLevels = (int)math.log2(sourceMaxWidth) + 1;

        int destinationMaxWidth = math.max(math.max(destinationDesc.width, destinationDesc.height), destinationDesc.slices);
        int destinationTotalMipChainLevels = (int)math.log2(destinationMaxWidth) + 1;

        if (numSlices == -1) numSlices = sourceDesc.slices - sourceSlice;
        if (numSlices > sourceDesc.slices - sourceSlice
            || numSlices > destinationDesc.slices - destinationSlice) {
            throw new ArgumentException($"BlitPass: {passName} attempts to blit too many slices. The pass will be skipped.");
        }
        if (numMips == -1) numMips = sourceTotalMipChainLevels - sourceMip;
        if (numMips > sourceTotalMipChainLevels - sourceMip
            || numMips > destinationTotalMipChainLevels - destinationMip) {
            throw new ArgumentException($"BlitPass: {passName} attempts to blit too many mips. The pass will be skipped.");
        }

        using (var builder = graph.AddUnsafePass<BlitPassData>(passName, out var passData, file, line)) {
            passData.source = source;
            passData.destination = destination;
            passData.scale = scale;
            passData.offset = offset;
            passData.sourceSlice = sourceSlice;
            passData.destinationSlice = destinationSlice;
            passData.numSlices = numSlices;
            passData.sourceMip = sourceMip;
            passData.destinationMip = destinationMip;
            passData.numMips = numMips;
            passData.filterMode = filterMode;
            passData.isXR = IsTextureXR(ref destinationDesc, sourceSlice, destinationSlice, numSlices, numMips);
            builder.UseTexture(source, AccessFlags.Read);
            builder.UseTexture(destination, AccessFlags.Write);
            builder.SetRenderFunc((BlitPassData data, UnsafeGraphContext context) => BlitRenderFunc(data, context));
        }
    }

    static Vector4 s_BlitScaleBias = new Vector4();
    static void BlitRenderFunc(BlitPassData data, UnsafeGraphContext context)
    {
        s_BlitScaleBias.x = data.scale.x;
        s_BlitScaleBias.y = data.scale.y;
        s_BlitScaleBias.z = data.offset.x;
        s_BlitScaleBias.w = data.offset.y;

        CommandBuffer unsafeCmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
        if (data.isXR) {
            // This is the magic that makes XR work for blit. We set the rendertargets passing -1 for the slices. This means it will bind all (both eyes) slices. 
            // The engine will then also automatically duplicate our draws and the vertex and pixel shader (through macros) will ensure those draws end up in the right eye.
            context.cmd.SetRenderTarget(data.destination, 0, CubemapFace.Unknown, -1);
            Blitter.BlitTexture(unsafeCmd, data.source, s_BlitScaleBias, data.sourceMip, data.filterMode == BlitFilterMode.ClampBilinear);
        } else {
            for (int currSlice = 0; currSlice < data.numSlices; currSlice++) {
                for (int currMip = 0; currMip < data.numMips; currMip++) {
                    context.cmd.SetRenderTarget(data.destination, data.destinationMip + currMip, CubemapFace.Unknown, data.destinationSlice + currSlice);

                    //(float)(data.sourceSlice + currSlice)
                    //Blitter.BlitTexture(unsafeCmd, data.source, s_BlitScaleBias, data.sourceMip + currMip, data.filterMode == BlitFilterMode.ClampBilinear);
                    //Blitter.BlitTexture(unsafeCmd, data.source, s_BlitScaleBias, data.sourceMip + currMip, data.sourceSlice + currSlice, data.filterMode == BlitFilterMode.ClampBilinear);
                }
            }
        }
    }

    internal static bool IsTextureXR(ref TextureDesc destDesc, int sourceSlice, int destinationSlice, int numSlices, int numMips)
    {
        if (TextureXR.useTexArray &&
              destDesc.dimension == TextureDimension.Tex2DArray &&
              destDesc.slices == TextureXR.slices &&
              sourceSlice == 0 &&
              destinationSlice == 0 &&
              numSlices == TextureXR.slices &&
              numMips == 1) {
            return true;
        }
        return false;
    }
}
