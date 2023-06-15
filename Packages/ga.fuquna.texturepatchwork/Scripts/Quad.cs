using System;
using UnityEngine;

namespace TexturePatchwork
{
    /// <summary>
    /// Rect to rotate in min
    /// </summary>
    [Serializable]
    public class Quad
    {
        public Vector2 leftBottom = Vector2.zero;
        public Vector2 leftTop = Vector2.up;
        public Vector2 rightBottom = Vector2.right;
        public Vector2 rightTop = Vector2.one;

        public static implicit operator Quad(Rect rect) => new()
        {
            leftBottom = rect.min,
            leftTop = new Vector2(rect.xMin, rect.yMax),
            rightBottom = new Vector2(rect.xMax, rect.yMin),
            rightTop = rect.max
        };
    }
}