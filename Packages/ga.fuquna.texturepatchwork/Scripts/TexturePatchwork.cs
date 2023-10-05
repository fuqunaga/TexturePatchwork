using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TexturePatchwork
{
    public static class TexturePatchwork
    {
        private static class ShaderParam
        {
            public static readonly int DstFactor = Shader.PropertyToID("_DstFactor");
            public static readonly int Homography = Shader.PropertyToID("_Homography");
        }
        
        private static Material _material;

        public static Material Material
        {
            get => _material ? _material : _material = new Material(Shader.Find("Hidden/TexturePatchwork"));
            set => _material = value;
        }

        public static void Render(RenderTexture targetTex, IEnumerable<PatchParameter> patchParameters, Color? clearColor = null, BlendMode dstBlendMode = BlendMode.Zero, bool useHomography = true)
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

            if (useHomography)
            {
                RenderWithHomography(mat, patchParameters);
            }
            else
            {
                RenderSimple(mat, patchParameters);
            }
        
            GL.PopMatrix();
        }
        
        private static void RenderWithHomography(Material material, IEnumerable<PatchParameter> patchParameters)
        {
            foreach (var patchParameter in patchParameters)
            {
                var readUV = patchParameter.readUV;
                var writeUV = patchParameter.writeUV;
                var writeLeftBottom = writeUV.leftBottom;
                var writeLeftTop = writeUV.leftTop;
                var writeRightTop = writeUV.rightTop;
                var writeRightBottom = writeUV.rightBottom;

                material.mainTexture = patchParameter.readTexture;
                material.SetMatrix(ShaderParam.Homography, CalcHomographyMatrix(writeUV, readUV));

                material.SetPass(0);

                GL.Begin(GL.QUADS);

                GL.Vertex3(writeLeftBottom.x, writeLeftBottom.y, 0f);
                GL.Vertex3(writeLeftTop.x, writeLeftTop.y, 0f);
                GL.Vertex3(writeRightTop.x, writeRightTop.y, 0f);
                GL.Vertex3(writeRightBottom.x, writeRightBottom.y, 0f);

                GL.End();
            }
        }

        private static void RenderSimple(Material material, IEnumerable<PatchParameter> patchParameters)
        {
            foreach (var patchParameter in patchParameters)
            {
                material.mainTexture = patchParameter.readTexture;
                material.SetPass(1);

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
        }
        
        public static Matrix4x4 CalcHomographyMatrix(Quad from, Quad to)
        {
            var unitToFrom = CalcHomographyMatrixFromUnitSquare(from);
            var unitToTo = CalcHomographyMatrixFromUnitSquare(to);
            
            return unitToTo * unitToFrom.inverse;
        }
        
        // (0,0),(1,1)の正方形がquadになるような変換行列を求める
        // https://qiita.com/fuqunaga/items/f1534b50ba483e884715
        public static Matrix4x4 CalcHomographyMatrixFromUnitSquare(Quad quad)
        {
            var p0 = quad.leftBottom;
            var p1 = quad.leftTop;
            var p2 = quad.rightTop;
            var p3 = quad.rightBottom;
            
            var sx = p0.x - p1.x + p2.x - p3.x;
            var sy = p0.y - p1.y + p2.y - p3.y;

            var dx1 = p1.x - p2.x;
            var dx2 = p3.x - p2.x;
            var dy1 = p1.y - p2.y;
            var dy2 = p3.y - p2.y;

            var z = (dy1 * dx2) - (dx1 * dy2);
            var g = ((sx * dy1) - (sy * dx1)) / z;
            var h = ((sy * dx2) - (sx * dy2)) / z;

            ReadOnlySpan<float> system = stackalloc []{
                p3.x * g - p0.x + p3.x,
                p1.x * h - p0.x + p1.x,
                p0.x,
                p3.y * g - p0.y + p3.y,
                p1.y * h - p0.y + p1.y,
                p0.y,
                g,
                h,
            };

            var mtx = Matrix4x4.identity;
            mtx.m00 = system[0]; mtx.m01 = system[1]; mtx.m02 = system[2];
            mtx.m10 = system[3]; mtx.m11 = system[4]; mtx.m12 = system[5];
            mtx.m20 = system[6]; mtx.m21 = system[7]; mtx.m22 = 1f;

            return mtx;
        }
    }
}