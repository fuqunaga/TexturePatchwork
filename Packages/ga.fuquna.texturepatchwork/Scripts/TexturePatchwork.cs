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

        public static void Render(RenderTexture targetTex, IEnumerable<PatchParameter> patchParameters, Color? clearColor = null, BlendMode dstBlendMode = BlendMode.Zero)
        {
            if (patchParameters == null ) return;
            
            var mat = Material;
            mat.SetInt(ShaderParam.DstFactor, (int)dstBlendMode);

            Graphics.SetRenderTarget(targetTex);
            if (clearColor != null)
            {
                GL.Clear(true, true, clearColor.Value);
            }

            GL.PushMatrix();
            GL.LoadOrtho();

            foreach(var patchParameter in patchParameters)
            {
                mat.mainTexture = patchParameter.readTexture;
                mat.SetPass(0);

                var readUV = patchParameter.readUV;
                var readLeftBottom = readUV.leftBottom;
                var readLeftTop = readUV.leftTop;
                var readRightTop = readUV.rightTop;
                var readRightBottom = readUV.rightBottom;

                var writeUV = patchParameter.writeUV;
                var writeLeftBottom = writeUV.leftBottom;
                var writeLeftTop = writeUV.leftTop;
                var writeRightTop = writeUV.rightTop;
                var writeRightBottom = writeUV.rightBottom;

                GL.Begin(GL.QUADS);

                GL.TexCoord2(readLeftBottom.x, readLeftBottom.y);
                GL.Vertex3(writeLeftBottom.x, writeLeftBottom.y, 0f);

                GL.TexCoord2(readLeftTop.x, readLeftTop.y);
                GL.Vertex3(writeLeftTop.x, writeLeftTop.y, 0f);

                GL.TexCoord2(readRightTop.x, readRightTop.y);
                GL.Vertex3(writeRightTop.x, writeRightTop.y, 0f);

                GL.TexCoord2(readRightBottom.x, readRightBottom.y);
                GL.Vertex3(writeRightBottom.x, writeRightBottom.y, 0f);

                GL.End();
            }

            GL.PopMatrix();
        }
    }
}