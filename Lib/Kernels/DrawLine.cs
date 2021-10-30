
using UnityEngine;

namespace Kernels
{
    internal class DrawLineKernel
    {

        static readonly string NAME = "DrawLine";

        readonly ComputeShader shader;
        int kernelIndex;
        uint shaderX;
        uint shaderY;
        uint shaderZ;

        public DrawLineKernel(ComputeShader shader)
        {
            this.shader = shader;
            kernelIndex = shader.FindKernel(NAME);
            shader.GetKernelThreadGroupSizes(kernelIndex, out shaderX, out shaderY, out shaderZ);
        }

        public RenderTexture Compute(Texture texture, Vector2 before, Vector2 after)
        {
            var result = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true
            };
            result.Create();

            shader.SetTexture(kernelIndex, "image", texture);
            shader.SetTexture(kernelIndex, "result", result);
            shader.SetFloat("decaySpeed", Time.deltaTime * 3);
            shader.SetFloat("prevX", (int)before.x);
            shader.SetFloat("prevY", (int)before.y);
            shader.SetFloat("inputX", (int)after.x);
            shader.SetFloat("inputY", (int)after.y);

            shader.Dispatch(kernelIndex, texture.width / (int)shaderX, texture.height / (int)shaderY, (int)shaderZ);

            return result;

        }
    }
}
