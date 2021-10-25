#nullable enable

using UnityEngine;

namespace Kernels
{
    internal class MergeIconsKernel
    {

        static readonly string NAME = "MergeIcons";

        readonly ComputeShader shader;
        int kernelIndex;
        uint shaderX;
        uint shaderY;
        uint shaderZ;

        public MergeIconsKernel(ComputeShader shader)
        {
            this.shader = shader;
            kernelIndex = shader.FindKernel(NAME);
            shader.GetKernelThreadGroupSizes(kernelIndex, out shaderX, out shaderY, out shaderZ);
        }

        public RenderTexture Compute(Icon r, Icon g, Icon b)
        {
            var result = new RenderTexture(256,256, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true
            };
            result.Create();

            shader.SetTexture(kernelIndex, "R", r.texture);
            shader.SetTexture(kernelIndex, "G", g.texture);
            shader.SetTexture(kernelIndex, "B", b.texture);
            shader.SetTexture(kernelIndex, "Result", result);
            shader.SetFloat("Rrot", r.rotation);
            shader.SetFloat("Grot", g.rotation);
            shader.SetFloat("Brot", b.rotation);

            shader.Dispatch(kernelIndex, 256 / (int)shaderX, 256 / (int)shaderY, (int)shaderZ);

            return result;

        }
    }
}
