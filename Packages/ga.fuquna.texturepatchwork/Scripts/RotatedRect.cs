using System;
using UnityEngine;

namespace TexturePatchwork
{
    /// <summary>
    /// Rect to rotate in min
    /// </summary>
    [Serializable]
    public class RotatedRect
    {
        public Rect rect = new(Vector2.zero, Vector2.one);
        public float rotate;
        
        public Vector2 LeftBottom => rect.min;
        public Vector2 LeftTop => ApplyRotate(new Vector2(rect.xMin, rect.yMax));
        public Vector2 RightBottom => ApplyRotate(new Vector2(rect.xMax, rect.yMin));
        public Vector2 RightTop => ApplyRotate(new Vector2(rect.xMax, rect.yMax));
        
        private Vector2 ApplyRotate(Vector2 v)
        {
            v -= rect.min;
            var rad = rotate * Mathf.Deg2Rad;
            var sin = Mathf.Sin(rad);
            var cos = Mathf.Cos(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos) + rect.min;
        }
    }
}