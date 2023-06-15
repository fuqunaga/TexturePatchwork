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
    }
}