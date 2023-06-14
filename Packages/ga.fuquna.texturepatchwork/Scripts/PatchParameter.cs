using System;
using UnityEngine;

namespace TexturePatchwork
{
    [Serializable]
    public class PatchParameter
    {
        public Texture tex;
     
        public Vector2 readLeftBottom = Vector2.zero;
        public Vector2 readLeftTop = new(0f, 1f);
        public Vector2 readRightTop = new(1f, 1f);
        public Vector2 readRightBottom = new(1f, 0f);
        
        public Rect writeRect = new(Vector2.zero, Vector2.one);

        public void SetRead(Rect rect)
        {
            readLeftBottom = rect.min;
            readLeftTop = new Vector2(rect.xMin, rect.yMax);
            readRightTop = rect.max;
            readRightBottom = new Vector2(rect.xMax, rect.yMin);
        }
    }
}