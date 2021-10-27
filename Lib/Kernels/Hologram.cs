using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kernels
{
    class HologramKernel
    {
        static readonly string NAME = "Hologram";

        readonly ComputeShader shader;
        int kernelIndex;
        uint shaderX;
        uint shaderY;
        uint shaderZ;

        public HologramKernel(ComputeShader shader)
        {
            this.shader = shader;
            kernelIndex = shader.FindKernel(NAME);
            shader.GetKernelThreadGroupSizes(kernelIndex, out shaderX, out shaderY, out shaderZ);
        }

        public RenderTexture Compute(Texture texture, int horizRandStrength)
        {
            var result = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true
            };
            result.Create();

            shader.SetTexture(kernelIndex, "Image", texture);
            shader.SetTexture(kernelIndex, "Result", result);
            shader.SetFloat("Time", Time.realtimeSinceStartup);
            shader.SetInt("W", texture.width);
            shader.SetInt("H", texture.height);
            shader.SetInt("HorizRandStrength", horizRandStrength);

            shader.Dispatch(kernelIndex, texture.width / (int)shaderX, texture.height / (int)shaderY, (int)shaderZ);

            return result;

        }
    }
}


