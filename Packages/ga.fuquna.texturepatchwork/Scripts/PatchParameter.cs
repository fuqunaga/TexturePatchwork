using System;
using UnityEngine;

namespace TexturePatchwork
{
    [Serializable]
    public class PatchParameter
    {
        public Texture readTexture;
        public Quad readUV = new();
        public Quad writeUV = new();
    }
}