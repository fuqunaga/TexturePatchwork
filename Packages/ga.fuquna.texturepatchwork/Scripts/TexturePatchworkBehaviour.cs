using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace TexturePatchwork
{
    /// <summary>
    /// テクスチャの一部分を切り貼りして新しいテクスチャを生成する
    /// </summary>
    public class TexturePatchworkBehaviour : MonoBehaviour
    {
        public enum UpdateMode
        {
            EveryFrame,
            Lazy
        }
        
        public Color clearColor = Color.black;
        public BlendMode dstBlendMode = BlendMode.Zero;
        [Tooltip("Textures connect smoothly across polygons.")]
        public bool useHomography = true;
        public Vector2Int targetTextureSize = new(1920, 1080);
        public List<PatchParameter> patchParameters = new();
        public UpdateMode updateMode = UpdateMode.EveryFrame;

        public int customMaterialPass = -1;
        public event Action prepareRender;
        
        [Tooltip("For debug preview")] [SerializeField]
        private RenderTexture targetTexture;

        private bool _needUpdate = true;

        public RenderTexture Texture
        {
            get
            {
                if (_needUpdate)
                {
                    UpdateTex();
                    _needUpdate = false;
                }

                return targetTexture;
            }
        }

        public bool IsValid => patchParameters.All(rt => rt.readTexture != null);


        private void Update()
        {
            if (updateMode == UpdateMode.EveryFrame)
            {
                UpdateTex();
            }
            else
            {
                _needUpdate = true;
            }
        }

        private void UpdateTex()
        {
            CheckTex();
            
            prepareRender?.Invoke();
            var pass = customMaterialPass >= 0 
                ? customMaterialPass 
                : (useHomography ? 0 : 1);
            TexturePatchwork.Render(targetTexture, patchParameters, clearColor, dstBlendMode, useHomography, pass);
        }

        private void CheckTex()
        {
            if (targetTexture == null || (targetTexture.width != targetTextureSize.x) || (targetTexture.height != targetTextureSize.y))
            {
                if (targetTexture != null) Destroy(targetTexture);
                targetTexture = new RenderTexture(targetTextureSize.x, targetTextureSize.y, 0);
                targetTexture.Create();
            }
        }
    }
}