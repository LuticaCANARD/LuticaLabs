using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static LuticaSKID.Models.TextureImageProcessing;
using static LuticaSKID.StructTypes;
using static LuticaLab.SKIDUnityConverter;
using static LuticaLab.LuticaLabLogger;
using LuticaLab.MeshMetro;
using LuticaSKID;
using static LuticaSKID.Models.TextureImageProcessing.ImageProcessType;
namespace LuticaLab.MeshMetro
{
    public abstract class SKIDFlow : ScriptableObject
    {
        public abstract ImageProcessType GetOption();
        public abstract ImageProcessCommandOrder Order { get; set; }
        public abstract float Scale { get; set; }
    }
    public class SKIDTextureWorkflow : SKIDFlow
    {
        public Texture2D referenceTexture;
        private ImageProcessCommandOrder order;
        private float scale;

        public SKIDTextureWorkflow(Texture2D referenceTexture, ImageProcessCommandOrder order, float scale)
        {
            this.referenceTexture = referenceTexture;
            this.order = order;
            this.scale = scale;
        }

        public override ImageProcessCommandOrder Order
        {
            get => order;
            set => order = value;
        }

        public override float Scale
        {
            get => scale;
            set => scale = value;
        }

        public Vector2Int SetPoint { get; set; }
        public Vector2Int Size { get; set; }
        public bool AutoCenterSelect { get; set; }
        public float Angle { get; set; }
      
        public override ImageProcessType GetOption()
        {
            return GenerateToCommandOption(order, scale, referenceTexture);
        }
    }
    class SKIDTextureWorkflowSingle : SKIDFlow
    {
        private ImageProcessCommandOrder order;
        private float scale;

        public SKIDTextureWorkflowSingle(ImageProcessCommandOrder order, float scale)
        {
            this.order = order;
            this.scale = scale;
        }

        public override ImageProcessCommandOrder Order
        {
            get => order;
            set => order = value;
        }

        public override float Scale
        {
            get => scale;
            set => scale = value;
        }

        public override ImageProcessType GetOption()
        {
            return GenerateToCommandOption(order, scale, null);
        }
    }
    public class MeshTextureProcessor : MeshMetroEditorContent
    {
        private Mesh referenceMesh;
        private Texture2D originalTexture;
        private Texture2D tmp_referenceTexture;

        private List<SKIDFlow> orders = new();
        private SKIDFlow tempOrder;

        private ImageProcessCommandOrder temp_order;
        private bool toggleOrder;
        private bool temp_is_need_two;
        private float temp_scale;
        private Vector2 scrolling_orders;

        private Texture2D processedTexture;
        private bool genetareNormal;
        private float x_normal;
        private float y_normal;
        private Texture2D processedNormal;
        private bool generateHeight;
        private Texture2D processedHeight;
        private bool generateOcclusion;
        private Texture2D processedOcclusion;
        private bool generateMetallic;
        private Texture2D processedMetallic;
        private bool generateRoughness;
        private Texture2D processedRoughness;

        public override void ShowOnMetro()
        {
            if (tempOrder == null)
            {
                tempOrder = ScriptableObject.CreateInstance<SKIDTextureWorkflowSingle>();
            }

            EditorGUILayout.LabelField("Mesh Texture Processor", EditorStyles.boldLabel);
            CreateField("Reference Mesh", ref referenceMesh);
            CreateTextureField("Original Texture", ref originalTexture);

            toggleOrder = EditorGUILayout.Foldout(toggleOrder, $"Show Order Queue Count:{orders.Count}");
            if (toggleOrder)
            {
                ShowOrderQueue();
            }

            ShowTempOrder();
            ShowGenerateOptions();

            if (GUILayout.Button("Execute"))
            {
                ProcessExecute();
            }

            ShowAllPreview();
        }

        private void ShowOrderQueue()
        {
            EditorGUILayout.LabelField($"Order Queue (Now Cound:{orders.Count})", EditorStyles.boldLabel);
            scrolling_orders = EditorGUILayout.BeginScrollView(scrolling_orders, GUILayout.Height(200));
            for (int i = 0; i < orders.Count; i++)
            {
                EditorGUILayout.LabelField($"Order {i + 1}: {orders[i].GetType().Name}");
                ShowCurrentOrder(orders[i]);
                if (GUILayout.Button($"Remove Order {i + 1}"))
                {
                    orders.RemoveAt(i);
                    break;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void ShowTempOrder()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add Order");

            var need_new = EditorGUILayout.Toggle(temp_is_need_two);
            if (need_new != temp_is_need_two)
            {
                temp_is_need_two = need_new;
                if (temp_is_need_two)
                {
                    tempOrder = ScriptableObject.CreateInstance<SKIDTextureWorkflow>();
                }
                else
                {
                    tempOrder = ScriptableObject.CreateInstance<SKIDTextureWorkflowSingle>();
                }

            }
            if (temp_is_need_two)
            {
                var v = tempOrder as SKIDTextureWorkflow;
                CreateTextureField("Reference Texture", ref v.referenceTexture);
            }
            tempOrder.Order = (ImageProcessCommandOrder)EditorGUILayout.EnumPopup("Process Type", tempOrder.Order);
            temp_scale = EditorGUILayout.FloatField("Scale", temp_scale);

            if (GUILayout.Button("Add"))
            {
                orders.Add(tempOrder);
                tmp_referenceTexture = null;
                tempOrder = temp_is_need_two
                    ? ScriptableObject.CreateInstance<SKIDTextureWorkflow>()
                    : ScriptableObject.CreateInstance<SKIDTextureWorkflowSingle>();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowCurrentOrder(SKIDFlow order)
        {
            EditorGUILayout.LabelField($"Order Type: {order.Order}");
            EditorGUILayout.LabelField($"Scale: {order.Scale}");
            if (order is SKIDTextureWorkflow workflow)
            {
                CreateTextureField("Reference Texture", ref workflow.referenceTexture);
            }
        }

        private void ShowGenerateOptions()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Generate Normal Map");
            genetareNormal = EditorGUILayout.Toggle(genetareNormal);
            EditorGUILayout.EndHorizontal();

            if (genetareNormal)
            {
                EditorGUILayout.BeginHorizontal();
                x_normal = EditorGUILayout.FloatField("X Normal", x_normal);
                y_normal = EditorGUILayout.FloatField("Y Normal", y_normal);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Generate Height Map");
            generateHeight = EditorGUILayout.Toggle(generateHeight);
            EditorGUILayout.EndHorizontal();
        }

        private void GenerateNormalMap(SKIDImage img)
        {
            if (originalTexture == null)
            {
                Debug.LogError("Reference Mesh or Original Texture is null");
                return;
            }

            if (!originalTexture.isReadable)
            {
                LuticaLabFolderManager.AssetSetReadWrite(originalTexture);
            }

            var cmd = ImageProcessCommand.NewGenerateNormalMap(
                new ImageProcessInput<NormalModule.NormalMapConfig>(
                    img,
                    new NormalModule.NormalMapConfig(x_normal, y_normal)
                )
            );

            var processedImage = Generator.Process(cmd);
            processedNormal = ConvertToTexture(processedImage);
        }

        private void ProcessExecute()
        {
            if (originalTexture == null)
            {
                Debug.LogError("Reference Mesh or Original Texture is null");
                return;
            }

            if (!originalTexture.isReadable)
            {
                LuticaLabFolderManager.AssetSetReadWrite(originalTexture);
            }

            var imageInte = ConvertTexture(originalTexture);
            foreach (var order in orders)
            {
                var option = order.GetOption();
                var imageProcessInputOption = new ImageProcessInputOption(option, order.Scale);
                var cmd2 = new ImageProcessInput<ImageProcessInputOption>(imageInte, config: imageProcessInputOption);
                if (order is SKIDTextureWorkflowSingle)
                {
                    imageInte = Generator.Process(ImageProcessCommand.NewProcessImage(cmd2));
                }
                else if (order is SKIDTextureWorkflow)
                {
                    var generated = order as SKIDTextureWorkflow;
                    var opt = generated.GetOption();
                    if (opt is not TwoImageProcess f)
                    {
                        Debug.LogError("Invalid order type");
                        return;
                    }
                    if(!generated.referenceTexture.isReadable)
                    {
                        LuticaLabFolderManager.AssetSetReadWrite(generated.referenceTexture);
                    }
                    imageInte = Generator.Process(ImageProcessCommand.NewProcessImageWithPartial(
                        new ImageProcessInput<SimpleImageSynArgu>(
                            imageInte,
                            new SimpleImageSynArgu(
                                f.Item1, // Added required parameter 'processType'
                                new BoxedZoneEditAdaptor.MarkingImage(
                                    ConvertTexture(generated.referenceTexture),
                                    new SKIDPixelVector2(generated.Size.x, generated.Size.y),
                                    BoxedZoneEditAdaptor.StickingType.PreferenceStickingSize,
                                    new SKIDPixelVector2(generated.SetPoint.x, generated.SetPoint.y),
                                    generated.Angle
                                ),
                                generated.Scale
                            )
                        )
                    ));
                }
            }

            if (genetareNormal)
            {
                GenerateNormalMap(imageInte);
            }

            processedTexture = ConvertToTexture(imageInte);
        }

        private void ShowAllPreview()
        {
            ShowTexturePreview(processedTexture);
            ShowTexturePreview(processedNormal);
            ShowTexturePreview(processedHeight);
            ShowTexturePreview(processedOcclusion);
            ShowTexturePreview(processedMetallic);
            ShowTexturePreview(processedRoughness);
        }

        private void ShowTexturePreview(Texture2D texture)
        {
            if (texture != null)
            {
                CreateTextureDisplayField(texture, GUILayout.Width(200), GUILayout.Height(200));
            }
        }
    }
}