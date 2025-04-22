using LuticaLab.MeshMetro;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static LuticaLab.LuticaLabLogger;
using static LuticaSKID.Models.TextureImageProcessing;
using static LuticaSKID.StructTypes;
using static LuticaLab.SKIDUnityConverter;
using LuticaSKID;

namespace LuticaLab.MeshMetro
{
    public class ImageSynthesisWindow : MeshMetroEditorContent
    {
        Texture2D originTexture;
        Texture2D resultTexture;
        Texture2D maskTexture;

        Vector2 scrollPositionOnResult;
        bool showOptions;

        Vector2Int maskCenter;
        Vector2Int maskSize;
        ImageProcessTwoImage operation;
        bool need_reset;
        bool auto_calculate;
        float filterConstant;
        bool initialized;
        float angle;
        public override void ShowOnMetro()
        {
            EditorGUILayout.LabelField("Image Synthesis", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            CreateField("Origin Texture", ref originTexture);
            CreateField("Mask Texture", ref maskTexture);
            if(!initialized)
            {
                initialized = true;
                auto_calculate = true;
                filterConstant = 1.0f;
                angle = 0f;
            }

            

            showOptions = EditorGUILayout.Foldout(showOptions,"Options...");
            if (showOptions)
            {
                EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

                operation = (ImageProcessTwoImage)EditorGUILayout.EnumPopup("Operation", operation);
                auto_calculate = EditorGUILayout.Toggle("Auto Calculate", auto_calculate);
                maskCenter = EditorGUILayout.Vector2IntField("Mask Center", maskCenter);
                GUILayout.Label("Filter Constant");
                filterConstant = EditorGUILayout.FloatField(filterConstant);
                angle = EditorGUILayout.FloatField(angle);
                if (!auto_calculate)
                {
                    maskSize = EditorGUILayout.Vector2IntField("Mask Size", maskSize);
                }
                else if(maskTexture)
                {
                    maskSize = new Vector2Int(maskTexture.width, maskTexture.height);
                    EditorGUILayout.Vector2IntField("Mask Size(Auto...)", maskSize);
                    EditorGUILayout.Vector2IntField("Mask Size(Auto...)", new Vector2Int(originTexture.width, originTexture.height));
                }
            }

            if (GUILayout.Button("Synthesize"))
            {
                if (originTexture != null && maskTexture != null)
                {
                    if (!originTexture.isReadable)
                        LuticaLabFolderManager.AssetSetReadWrite(originTexture);
                    if(!maskTexture.isReadable)
                        LuticaLabFolderManager.AssetSetReadWrite(maskTexture);
                    var generatedMask = ConvertTexture(maskTexture);
                    var marker = new BoxedZoneEditAdaptor.MarkingImage(
                        generatedMask,
                        new SKIDPixelVector2(maskSize.x, maskSize.y),
                        BoxedZoneEditAdaptor.StickingType.PreferenceStickingSize, // Added required parameter 'stickingType'
                        new SKIDPixelVector2(maskCenter.x, maskCenter.y), // Added required parameter 'center'
                        angle // Added required parameter 'angle'
                    );
                    var processCommand = new ImageProcessInput<SimpleImageSynArgu>(
                        ConvertTexture(originTexture),
                        new SimpleImageSynArgu(
                            operation, // Added required parameter 'processType'
                            marker, // Added required parameter 'partialImage'
                            filterConstant // Existing parameter 'constant'
                        ));
                    var cmd = ImageProcessCommand.NewProcessImageWithPartial(
                        processCommand

                    );
                    this.resultTexture = ConvertToTexture(generator.Process(cmd));
                }
                else
                {
                    Log("Please select both Origin and Mask textures.", EditorLogLevel.Warning);
                }
            }
            if (resultTexture != null)
            {
                scrollPositionOnResult = EditorGUILayout.BeginScrollView(scrollPositionOnResult);
                CreateTextureDisplayField(resultTexture, GUILayout.Width(resultTexture.width), GUILayout.Height(resultTexture.height));
                EditorGUILayout.EndScrollView();
                if(GUILayout.Button("Save Result Texture"))
                {
                    string path = EditorUtility.SaveFilePanel("Save Result Texture", "", "ResultTexture.png", "png");
                    if (!string.IsNullOrEmpty(path))
                    {
                        byte[] bytes = resultTexture.EncodeToPNG();
                        System.IO.File.WriteAllBytes(path, bytes);
                        AssetDatabase.Refresh();
                        Log("Result Texture Saved", EditorLogLevel.Info);
                    }
                }
            }
        }
    }
}
