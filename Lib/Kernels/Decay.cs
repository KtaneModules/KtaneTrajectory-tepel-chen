#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kernels
{
    internal class DecayKernel
    {

        static readonly string NAME = "Decay";

        readonly ComputeShader shader;
        int kernelIndex;
        uint shaderX;
        uint shaderY;
        uint shaderZ;

        public DecayKernel(ComputeShader shader)
        {
            this.shader = shader;
            kernelIndex = shader.FindKernel(NAME);
            shader.GetKernelThreadGroupSizes(kernelIndex, out shaderX, out shaderY, out shaderZ);
        }

        public RenderTexture Compute(Texture texture, float fade)
        {
            var result = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true
            };
            result.Create();

            shader.SetTexture(kernelIndex, "image", texture);
            shader.SetTexture(kernelIndex, "result", result);
            shader.SetFloat("decaySpeed", Time.deltaTime * 10);
            shader.SetFloat("fade", fade);

            shader.Dispatch(kernelIndex, texture.width / (int)shaderX, texture.height / (int)shaderY, (int)shaderZ);

            return result;

        }
    }
}
