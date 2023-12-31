﻿using UnityEngine;

namespace TexturePatchwork
{
    /// <summary>
    /// RectArrangementで生成したテクスチャをカメラの出力にする
    /// </summary>
    [RequireComponent(typeof(TexturePatchworkBehaviour), typeof(Camera))]
    public class TexturePatchworkBehaviourToCamera : MonoBehaviour
    {
        private static class ShaderParam
        {
            public static readonly int Enable = Shader.PropertyToID("_Enable");
            public static readonly int Texture = Shader.PropertyToID("_Texture");
        }

        public Material material;

        private TexturePatchworkBehaviour _patchworkBehaviour;
        private Camera _camera;
        private int _lastCullingMask;
        private CameraClearFlags _lastClearFlag;

        
        private void OnDisable()
        {
            if (_camera != null && _lastCullingMask != 0)
            {
                _camera.cullingMask = _lastCullingMask;
                _camera.clearFlags = _lastClearFlag;
            }

            material.SetFloat(ShaderParam.Enable, 0f);
        }

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _patchworkBehaviour = GetComponent<TexturePatchworkBehaviour>();
        }

        private void Update()
        {
            var enable = _patchworkBehaviour.isActiveAndEnabled && _patchworkBehaviour.IsValid;

            var isCameraEnable = _camera.cullingMask != 0;
            var change = (enable == isCameraEnable);
            if (change)
            {
                if (enable)
                {
                    _lastCullingMask = _camera.cullingMask;
                    _lastClearFlag = _camera.clearFlags;
                    _camera.cullingMask = 0;
                    _camera.clearFlags = CameraClearFlags.Nothing;

                    material.SetTexture(ShaderParam.Texture, _patchworkBehaviour.Texture);
                }
                else
                {
                    _camera.cullingMask = _lastCullingMask;
                    _camera.clearFlags = _lastClearFlag;
                }

                material.SetFloat(ShaderParam.Enable, enable ? 1f : 0f);
            }
        }
    }
}