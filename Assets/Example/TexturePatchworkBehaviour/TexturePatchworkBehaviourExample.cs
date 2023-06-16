using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace TexturePatchwork.Example
{
    [RequireComponent(typeof(TexturePatchworkBehaviour))]
    public class TexturePatchworkBehaviourExample : MonoBehaviour
    {
        [FormerlySerializedAs("rosettaUIRoot")] [SerializeField]
        private UIDocument uiDocument;

        private TexturePatchworkBehaviour _texturePatchworkBehaviour;


        private void Start()
        {
            _texturePatchworkBehaviour = GetComponent<TexturePatchworkBehaviour>();
            CreateUI();
        }

        private void Update()
        {
            var root = uiDocument.rootVisualElement;

            // To update the Image line, delete it once and add it again
            foreach (var image in root.Query<Image>().Build())
            {
                var parent = image.parent;
                image.RemoveFromHierarchy();
                parent.Add(image);
            }
        }

        private void CreateUI()
        {
            var root = uiDocument.rootVisualElement;

            var scrollView = root.Q("PatchworkParameterScrollView");
            var outputContainer = root.Q("OutputContainer");

            AddPatchworkParameterUI(scrollView);
            AddOutputUI(outputContainer);
        }

        private void AddPatchworkParameterUI(VisualElement container)
        {
            ForEachPatchParameterWithColor((patchParameter, color, index) =>
            {
                var label = new Label()
                {
                    text = $"Element{index}"
                };

                container.Add(label);
                container.Add(PatchworkParameterUI(patchParameter, color));
            });
        }

        private static VisualElement PatchworkParameterUI(PatchParameter patchParameter, Color lineColor)
        {
            var image = CreateHeightFitImage();
            image.image = patchParameter.readTexture;

            image.generateVisualContent += (context) => GenerateQuadLines(context, patchParameter.readUV, lineColor);

            var ve = new VisualElement();
            ve.Add(image);
            
            return ve;
        }

        private void AddOutputUI(VisualElement container)
        {
            var toggle = container.Q<Toggle>();
            
            var outputImage = CreateHeightFitImage();
            outputImage.generateVisualContent += GeneratePatchParametersQuadLines;
            outputImage.image = _texturePatchworkBehaviour.Texture;
            container.Add(outputImage);

            void GeneratePatchParametersQuadLines(MeshGenerationContext context)
            {
                if (!toggle.value) return;
                ForEachPatchParameterWithColor((parameter, color, _) =>
                {
                    GenerateQuadLines(context, parameter.writeUV, color);
                });
            }
        }

        private void ForEachPatchParameterWithColor(Action<PatchParameter, Color, int> action)
        {
            var patchParameters = _texturePatchworkBehaviour.patchParameters;
            var count = patchParameters.Count;
            for (var i = 0; i < count; ++i)
            {
                var color = Color.HSVToRGB((float)i / count, 1f, 1f);
                action(patchParameters[i], color, i);
            } 
        }


        private static void GenerateQuadLines(MeshGenerationContext context, Quad quad, Color color)
        {
            var rect = context.visualElement.layout;
            var painter = context.painter2D;
            painter.lineWidth = 3.0f;
            painter.lineCap = LineCap.Round;
            painter.lineJoin = LineJoin.Round;
            painter.strokeColor = color;
     
            painter.BeginPath();
            painter.MoveTo(UVToPixel(quad.leftBottom));
            painter.LineTo(UVToPixel(quad.leftTop));
            painter.LineTo(UVToPixel(quad.rightTop));
            painter.LineTo(UVToPixel(quad.rightBottom));
            painter.LineTo(UVToPixel(quad.leftBottom));
            painter.Stroke();
            
            Vector2 UVToPixel(Vector2 uv)
            {
                return new Vector2(uv.x, 1f - uv.y) * rect.size;
            }
        }
                


        private static Image CreateHeightFitImage()
        {
            var image = new Image();
            image.RegisterCallback<GeometryChangedEvent>((evt) =>
            {
                var tex = image.image;
                if (tex == null) return;

                image.style.height = evt.newRect.width * tex.height / tex.width;
            });

            return image;
        }
    }
}