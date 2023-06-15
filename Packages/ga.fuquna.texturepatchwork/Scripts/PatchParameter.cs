using System;
using UnityEngine;

namespace TexturePatchwork
{
    [Serializable]
    public class PatchParameter
    {
        public Texture readTexture;
        public RotatedRect readRectUv;
        public RotatedRect writeRectUv;
    }
}