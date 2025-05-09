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
using System.IO;
namespace LuticaLab.MeshMetro
{
    public abstract class SKIDFlow : ScriptableObject
    {
        public abstract ImageProcessType GetOption();
        public abstract ImageProcessCommandOrder Order { get; set; }
        public abstract float Volume { get; set; }
        public bool isShow;
    }
    public class SKIDTextureWorkflow : SKIDFlow
    {
        public Texture2D referenceTexture;
        private ImageProcessCommandOrder order;
        private float volume;

        public SKIDTextureWorkflow(Texture2D referenceTexture, ImageProcessCommandOrder order, float volume)
        {
            this.referenceTexture = referenceTexture;
            this.order = order;
            this.volume = volume;
            AutoCenterSelect = true;
            Size = new(referenceTexture.width,referenceTexture.height);

        }

        public override ImageProcessCommandOrder Order
        {
            get => order;
            set => order = value;
        }

        public override float Volume
        {
            get => volume;
            set => volume = value;
        }

        public Vector2Int SetPoint { get; set; }
        public Vector2Int Size { get; set; }
        public bool AutoCenterSelect { get; set; }
        public float Angle { get; set; }
        public bool AutoScale { get; set; }
      
        public override ImageProcessType GetOption()
        {
            return GenerateToCommandOption(order, Volume, referenceTexture);
        }
    }
    class SKIDTextureWorkflowSingle : SKIDFlow
    {
        private ImageProcessCommandOrder order;
        private float volume;

        public SKIDTextureWorkflowSingle(ImageProcessCommandOrder order, float volume)
        {
            this.order = order;
            this.volume = volume;
        }

        public override ImageProcessCommandOrder Order
        {
            get => order;
            set => order = value;
        }

        public override float Volume
        {
            get => volume;
            set => volume = value;
        }

        public override ImageProcessType GetOption()
        {
            return GenerateToCommandOption(order, volume, null);
        }
    }
    public class MeshTextureProcessor : MeshMetroEditorContent
    {
        const int MAX_ORDER_SHOW_HEIGHT = 500;
        private Mesh referenceMesh;
        private Texture2D originalTexture;
        private Texture2D tmp_referenceTexture;

        private List<SKIDFlow> orders = new();
        private SKIDFlow tempOrder;

        private bool toggleOrder = true;
        private bool temp_is_need_two;
        private float temp_volume;
        private Vector2 scrolling_orders;

        private Texture2D processedTexture;
        private bool genetareNormal;
        private float x_normal;
        private float y_normal;
        private Texture2D processedNormal;
        private bool generateHeight;
        private float x_height, y_height;
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
                tempOrder = CreateInstance<SKIDTextureWorkflowSingle>();
            }

            EditorGUILayout.LabelField(
                TranslateString("Mesh Texture Processor"), EditorStyles.boldLabel);
            CreateField(TranslateString("Reference Mesh"), ref referenceMesh);
            CreateTextureField(TranslateString("Original Texture"), ref originalTexture);
            ShowTempOrder();

            toggleOrder = EditorGUILayout.Foldout(toggleOrder, string.Format(TranslateString("Show Order Queue Count:{0}"), orders.Count));
            if (toggleOrder)
            {
                ShowOrderQueue();
            }

            ShowGenerateOptions();

            if (GUILayout.Button(TranslateString("Execute")))
            {
                ProcessExecute();
            }

            ShowAllPreview();
        }

        private void ShowOrderQueue()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            scrolling_orders = EditorGUILayout.BeginScrollView(scrolling_orders, GUILayout.Height(MAX_ORDER_SHOW_HEIGHT));
            for (int i = 0; i < orders.Count; i++)
            {
                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.BeginVertical();
                EditorGUILayout.LabelField(string.Format(TranslateString("Order {0}: {1}"), i + 1, orders[i].GetType().Name));

                ShowCurrentOrder(orders[i]);
                GUILayout.EndVertical();
                if (GUILayout.Button(string.Format(TranslateString("Remove Order {0}"),i+1)))
                {
                    orders.RemoveAt(i);
                }
                
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void ShowTempOrder()
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(TranslateString("Add Order"));
            
            GUILayout.Label("Command Type");
            var order_new = (ImageProcessCommandOrder)EditorGUILayout.EnumPopup(TranslateString("Process Type"), tempOrder.Order);
            tempOrder.Order = order_new;
            try // 뭔가 이유는 모르겠는데 포지션이 제대로 안잡힌다고 함.
            {
                if (temp_is_need_two == true)
                {
                    var v = tempOrder as SKIDTextureWorkflow;
                    CreateTextureField(TranslateString("Reference Texture"), ref tmp_referenceTexture);
                    v.referenceTexture = tmp_referenceTexture;
                    bool c = EditorGUILayout.Toggle(TranslateString("Auto Scale Set"), v.AutoScale);
                    v.AutoScale = c;
                    if (!v.AutoScale)
                    {
                        v.Size = EditorGUILayout.Vector2IntField(TranslateString("Image Size"), v.SetPoint);
                    }
                    v.AutoCenterSelect = EditorGUILayout.Toggle(TranslateString("Auto Center"), v.AutoCenterSelect);
                    if (!v.AutoCenterSelect)
                    {
                        v.SetPoint = EditorGUILayout.Vector2IntField(TranslateString("Set Point"), v.SetPoint);
                    }
                    v.Angle = EditorGUILayout.FloatField(TranslateString("Angle"), v.Angle);
                    temp_volume = EditorGUILayout.FloatField(TranslateString("Volume"), temp_volume);
                }
                else
                {
                    temp_volume = EditorGUILayout.FloatField(TranslateString("Volume"), temp_volume);
                }
            } 
            catch
            {

            }
            
            var need_new = CheckNeedTwoImage(order_new);
            if (need_new != temp_is_need_two)
            {
                temp_is_need_two = need_new;
                if (temp_is_need_two == true)
                {
                    tempOrder = CreateInstance<SKIDTextureWorkflow>();
                    var workflow = tempOrder as SKIDTextureWorkflow;
                    workflow.AutoScale = true;
                    workflow.AutoCenterSelect = true;
                }
                else
                {
                    tempOrder = CreateInstance<SKIDTextureWorkflowSingle>();
                }
                tempOrder.Order = order_new;
                tempOrder.Volume = temp_volume;


            }

            if (GUILayout.Button(TranslateString("Add")))
            {
                tempOrder.Order = order_new;
                tempOrder.Volume = temp_volume;

                if (temp_is_need_two == true && tmp_referenceTexture == null)
                {
                    Log("Reference Texture is not set", EditorLogLevel.Error);
                    return;
                } 
                else
                {
                    orders.Add(tempOrder);
                    temp_volume = 0;
                    tmp_referenceTexture = null;
                    tempOrder = temp_is_need_two
                        ? ScriptableObject.CreateInstance<SKIDTextureWorkflow>()
                        : ScriptableObject.CreateInstance<SKIDTextureWorkflowSingle>();
                    if (tempOrder is SKIDTextureWorkflow workflow)
                    {
                        workflow.AutoScale = true;
                        workflow.AutoCenterSelect = true;
                        temp_is_need_two = true;
                    }
                    else
                    {
                        temp_is_need_two = false;
                    }
                }
               
            }
            EditorGUILayout.EndVertical();



            GUILayout.EndHorizontal();
        }

        private void ShowCurrentOrder(SKIDFlow order)
        {
            EditorGUILayout.LabelField(
                string.Format(TranslateString("Order Type: {0}"), order.Order),EditorStyles.boldLabel);
            order.Volume = EditorGUILayout.FloatField(TranslateString("Volume"), order.Volume);
            if (order is SKIDTextureWorkflow workflow)
            {
                CreateTextureField(TranslateString("Reference Texture"), ref workflow.referenceTexture);
                bool zc = EditorGUILayout.Toggle(TranslateString("Auto Scale"),workflow.AutoScale);
                if (!workflow.AutoScale)
                {
                    workflow.Size = EditorGUILayout.Vector2IntField(TranslateString("Attach Image Size"), workflow.Size);
                }
                workflow.AutoCenterSelect = EditorGUILayout.Toggle(TranslateString("Auto Center Select"), workflow.AutoCenterSelect);
                if (!workflow.AutoCenterSelect)
                {
                    workflow.SetPoint = EditorGUILayout.Vector2IntField(TranslateString("Set Point"), workflow.SetPoint);
                }
                workflow.AutoScale = zc;
                workflow.Angle = EditorGUILayout.FloatField(TranslateString("Angle"), workflow.Angle);
            }
        }

        private void ShowGenerateOptions()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TranslateString("Generate Normal Map"));
            genetareNormal = EditorGUILayout.Toggle(genetareNormal);

            if (genetareNormal)
            {
                EditorGUILayout.BeginHorizontal();
                x_normal = EditorGUILayout.FloatField(string.Format(TranslateString("{0} Normal"), "X"), x_normal);
                y_normal = EditorGUILayout.FloatField(string.Format(TranslateString("{0} Normal"), "Y"), y_normal);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TranslateString("Generate Height Map"));
            generateHeight = EditorGUILayout.Toggle(generateHeight);
            if (generateHeight)
            {
                EditorGUILayout.BeginHorizontal();
                x_height = EditorGUILayout.FloatField(string.Format(TranslateString("{0} Height"), "X"), x_height);
                y_height = EditorGUILayout.FloatField(string.Format(TranslateString("{0} Height"), "Y"), y_height);
                EditorGUILayout.EndHorizontal();
            }
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

            var processedImage = Generator.Process(cmd, GPUAccelerator);
            processedNormal = ConvertToTexture(processedImage);
        }

        private void GenerateHeightMap(SKIDImage img)
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

            var cmd = ImageProcessCommand.NewProcessToHeightMap(
                new ImageProcessInput<LuticaSKID.Models.HeightMapModel.HeightMapConfig>(
                    img,
                    new LuticaSKID.Models.HeightMapModel.HeightMapConfig(x_height, y_height)
                )
            );

            var processedImage = Generator.Process(cmd, GPUAccelerator);
            processedHeight = ConvertToTexture(processedImage);
        }

        private void ProcessExecute()
        {
            if (originalTexture == null)
            {
                Log("originalTexture is not indecated.", EditorLogLevel.Error);
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
                var imageProcessInputOption = new ImageProcessInputOption(option, order.Volume);
                var cmd2 = new ImageProcessInput<ImageProcessInputOption>(imageInte, config: imageProcessInputOption);
                if (order is SKIDTextureWorkflowSingle)
                {
                    imageInte = Generator.Process(ImageProcessCommand.NewProcessImage(cmd2), GPUAccelerator);
                }
                else if (order is SKIDTextureWorkflow)
                {
                    var generated = order as SKIDTextureWorkflow;
                    var opt = generated.GetOption();
                    if (opt is not TwoImageProcess f)
                    {
                        Log("Invalid order type", EditorLogLevel.Error);
                        return;
                    }
                    if(!generated.referenceTexture.isReadable)
                    {
                        LuticaLabFolderManager.AssetSetReadWrite(generated.referenceTexture);
                    }
                    if(generated.AutoScale == true)
                    {
                        generated.Size = new Vector2Int(generated.referenceTexture.width, generated.referenceTexture.height);
                    }
                    if(generated.AutoCenterSelect == true)
                    {
                        generated.SetPoint = new Vector2Int(originalTexture.width / 2, originalTexture.height / 2);
                    }
                    Log($"size X:{generated.Size.x} size Y:{generated.Size.y} Angle {generated.Angle}");
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
                                generated.Volume
                            )
                        )
                    ), GPUAccelerator);
                }
            }

            if (genetareNormal)
            {
                GenerateNormalMap(imageInte);
            }
            if (generateHeight)
            {
                GenerateHeightMap(imageInte);
            }

            processedTexture = ConvertToTexture(imageInte);
        }

        private void ShowAllPreview()
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            ShowTexturePreview(processedTexture,"Generated Texture");
            ShowTexturePreview(processedNormal, "Generated Normal");
            ShowTexturePreview(processedHeight, "Generated Height");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUI.skin.box);
            ShowTexturePreview(processedOcclusion, "Generated Occlusion");
            ShowTexturePreview(processedMetallic, "Generated Metallic");
            ShowTexturePreview(processedRoughness, "Generated Roughness");
            GUILayout.EndHorizontal();
        }

        private void ShowTexturePreview(Texture2D texture,string name)
        {
            if (texture != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(name, EditorStyles.boldLabel);
                CreateTextureDisplayField(texture, GUILayout.Width(200), GUILayout.Height(200));
                if (GUILayout.Button("Save..."))
                {
                    string path = EditorUtility.SaveFilePanel("Save Generated Texture",
                        AssetDatabase.GetAssetPath(texture),
                        $"GeneratedTexture.png", "png");
                    if (!string.IsNullOrEmpty(path))
                    {
                        byte[] bytes = texture.EncodeToPNG();
                        File.WriteAllBytes(path, bytes);
                        AssetDatabase.Refresh();
                        Log("Generated texture saved to: " + path);
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}