#nullable enable

using UnityEngine;

namespace Kernels
{
    internal class ApplyMaskKernel
    {

        static readonly string NAME = "ApplyMask";

        readonly ComputeShader shader;
        int kernelIndex;
        uint shaderX;
        uint shaderY;
        uint shaderZ;

        public ApplyMaskKernel(ComputeShader shader)
        {
            this.shader = shader;
            kernelIndex = shader.FindKernel(NAME);
            shader.GetKernelThreadGroupSizes(kernelIndex, out shaderX, out shaderY, out shaderZ);
        }

        public RenderTexture Compute(Texture mask, Texture image, Vector4 color)
        {
            var result = new RenderTexture(256,256, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true
            };
            result.Create();

            shader.SetTexture(kernelIndex, "Mask", mask);
            shader.SetTexture(kernelIndex, "Image", image);
            shader.SetVector("Color", color);
            shader.SetTexture(kernelIndex, "Result", result);

            shader.Dispatch(kernelIndex, 256 / (int)shaderX, 256 / (int)shaderY, (int)shaderZ);

            return result;

        }
    }
}
