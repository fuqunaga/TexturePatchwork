using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace TexturePatchwork
{
    public static class TexturePatchwork
    {
        private static class ShaderParam
        {
            public static readonly int DstFactor = Shader.PropertyToID("_DstFactor");
        }
        
        private static Material _material;

        public static Material Material
        {
            get => _material ? _material : _material = new Material(Shader.Find("TexturePatchwork"));
            set => _material = value;
        }

        public static void Render(RenderTexture targetTex, List<PatchParameter> patchParameters, Color? clearColor = null, BlendMode dstBlendMode = BlendMode.Zero)
        {
            if (patchParameters == null || !patchParameters.Any()) return;
            
            var mat = Material;
            mat.SetInt(ShaderParam.DstFactor, (int)dstBlendMode);

            Graphics.SetRenderTarget(targetTex);
            if (clearColor != null)
            {
                GL.Clear(true, true, clearColor.Value);
            }

            GL.PushMatrix();
            GL.LoadOrtho();

            patchParameters.ForEach(refTex =>
            {
                mat.mainTexture = refTex.tex;
                mat.SetPass(0);

                var readLeftBottom = refTex.readLeftBottom;
                var readLeftTop = refTex.readLeftTop;
                var readRightTop = refTex.readRightTop;
                var readRightBottom = refTex.readRightBottom;

                var write = refTex.writeRect;

                GL.Begin(GL.QUADS);

                GL.TexCoord2(readLeftBottom.x, readLeftBottom.y);
                GL.Vertex3(write.min.x, write.min.y, 0f);

                GL.TexCoord2(readLeftTop.x, readLeftTop.y);
                GL.Vertex3(write.min.x, write.max.y, 0f);

                GL.TexCoord2(readRightTop.x, readRightTop.y);
                GL.Vertex3(write.max.x, write.max.y, 0f);

                GL.TexCoord2(readRightBottom.x, readRightBottom.y);
                GL.Vertex3(write.max.x, write.min.y, 0f);

                GL.End();
            });

            GL.PopMatrix();
        }
    }
}