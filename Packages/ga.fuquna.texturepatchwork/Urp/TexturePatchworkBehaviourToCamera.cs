using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TexturePatchwork.Urp
{
    /// <summary>
    /// TexturePatchworkBehaviourで生成したテクスチャをカメラの出力にする
    /// - メインカメラのRendererFeatureでFullScreenPassRendererFeatureを追加し、
    ///   そのPassMaterialにmaterialをセットして使うことを想定している
    /// - カメラ自体は他のオブジェクトを描画しないようにする
    /// </summary>
    [RequireComponent(typeof(TexturePatchworkBehaviour))]
    public class TexturePatchworkBehaviourToCamera : MonoBehaviour
    {
        private static class ShaderParam
        {
            public static readonly int Enable = Shader.PropertyToID("_Enable");
            public static readonly int Texture = Shader.PropertyToID("_Texture");
        }

        [SerializeField] private Camera targetCamera;
        [SerializeField] private Material material;

        private TexturePatchworkBehaviour _patchworkBehaviour;
        
        private int _lastCullingMask;
        private CameraClearFlags _lastClearFlag;


        private void Awake()
        {
            _patchworkBehaviour = GetComponent<TexturePatchworkBehaviour>();
        }

        private void OnEnable()
        {
            _lastCullingMask = targetCamera.cullingMask;
            _lastClearFlag = targetCamera.clearFlags;
            targetCamera.cullingMask = 0;
            targetCamera.clearFlags = CameraClearFlags.Nothing;
            
            material.SetFloat(ShaderParam.Enable, 1f);
        }

        private void OnDisable()
        {
            if (targetCamera != null && _lastCullingMask != 0)
            {
                targetCamera.cullingMask = _lastCullingMask;
                targetCamera.clearFlags = _lastClearFlag;
            }

            material.SetFloat(ShaderParam.Enable, 0f);
        }

#if UNITY_EDITOR
        private void Start()
        {
            CheckRendererFeature();
        }
#endif

        private void Update()
        {
            // _patchworkBehaviour.Textureが変わってる場合もあるので毎フレームセット
            material.SetTexture(ShaderParam.Texture, _patchworkBehaviour.Texture);
        }

        
#if UNITY_EDITOR
        // targetCameraにRendererFeatureにmaterialがセットされたFullScreenPassRendererFeatureが登録されているか確認する
        [ContextMenu("CheckRendererFeature")]
        private void CheckRendererFeature()
        {
            var rendererData = targetCamera.GetScriptableRendererData();
            var hasValidRendererFeature = rendererData.rendererFeatures
                .OfType<FullScreenPassRendererFeature>()
                .Any(rendererFeature => rendererFeature.passMaterial == material);

            if (!hasValidRendererFeature)
            {
                // ダイアログで警告を表示し、追加するか確認する
                var addFeature = EditorUtility.DisplayDialog(
                    $"{nameof(TexturePatchworkBehaviourToCamera)}",
                    "TexturePatchworkBehaviourToCamera はカメラのFullScreenPassRendererFeatureを利用してTexturePatchworkBehaviourの出力をカメラにBlitします。\n" +
                    "指定されたMaterialを使用するFullScreenPassRendererFeatureがカメラのRendererFeatureに登録されていません。\n" +
                    "追加しますか？",
                    "Add",
                    "Cancel"
                );

                if (addFeature)
                {
                    Undo.RecordObject(rendererData, "Add FullScreenPassRendererFeature");

                    var newFeature = ScriptableObject.CreateInstance<FullScreenPassRendererFeature>();
                    newFeature.name = material.name;
                    newFeature.injectionPoint = FullScreenPassRendererFeature.InjectionPoint.BeforeRenderingPostProcessing;
                    newFeature.passMaterial = material;
                    
                    rendererData.rendererFeatures.Add(newFeature);
                    
                    // newFeatureをrendererDataアセットに追加して保存
                    AssetDatabase.AddObjectToAsset(newFeature, rendererData);
                    AssetDatabase.SaveAssets();
                    EditorUtility.SetDirty(rendererData);
                    
                    EditorGUIUtility.PingObject(newFeature);
                }
            }
        }
#endif
    }
}