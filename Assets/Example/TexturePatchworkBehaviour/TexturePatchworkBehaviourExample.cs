using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace TexturePatchwork.Example
{
    [RequireComponent(typeof(TexturePatchworkBehaviour))]
    public class TexturePatchworkBehaviourExample : MonoBehaviour
    {
        [FormerlySerializedAs("rosettaUIRoot")] [SerializeField] private UIDocument uiDocument;
        private TexturePatchworkBehaviour _texturePatchworkBehaviour;
        
        
        private void Start()
        {
            _texturePatchworkBehaviour = GetComponent<TexturePatchworkBehaviour>();
            CreateUI();
        }


        private void CreateUI()
        {
            var root = uiDocument.rootVisualElement;
            var image = new Image()
            {
                image = _texturePatchworkBehaviour.Texture
            };
            root.Add(image);
        }
    }
}