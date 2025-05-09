#if UNITY_EDITOR
using LuticaLab.MeshMetro;
using LuticaSKID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LuticaSKID.Models.TextureImageProcessing;
using static LuticaSKID.StructTypes;
using static LuticaLab.SKIDUnityConverter;
using static LuticaLab.LuticaLabLogger;
using UnityEditor;

namespace LuticaLab.MeshMetro
{
    /// <summary>
    /// ImageProcessor for MeshMetro.
    /// </summary>
    public class ImageProcessor : MeshMetroEditorContent
    {

        private Texture2D texTarget;
        private Texture2D texRef;
        private Texture2D texResult;
        private ImageProcessCommandOrder processType;
        private float constantValue;
        private Vector2 ImageScrollVector;

        public override void ShowOnMetro()
        {
            EditorGUILayout.LabelField("Image Process", EditorStyles.boldLabel);
            texTarget = (Texture2D)EditorGUILayout.ObjectField("Target Texture", texTarget, typeof(Texture2D), false);
            processType = (ImageProcessCommandOrder)EditorGUILayout.EnumPopup("Process Type", processType);
            if (CheckNeedTwoImage(processType))
            {
                texRef = (Texture2D)EditorGUILayout.ObjectField("Reference Texture", texRef, typeof(Texture2D), false);
            }
            constantValue = EditorGUILayout.FloatField("Constant Value", constantValue);
            if (GUILayout.Button("Process"))
            {
                ProcessImage();
            }
            if(texResult != null)
            {
                GUILayout.Label("Processed Texture", EditorStyles.boldLabel);
                ImageScrollVector = EditorGUILayout.BeginScrollView(ImageScrollVector, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                CreateTextureDisplayField(texResult, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Save Processed Texture"))
                {
                    string path = EditorUtility.SaveFilePanel("Save Processed Texture", "", "ProcessedTexture.png", "png");
                    if (!string.IsNullOrEmpty(path))
                    {
                        byte[] bytes = texResult.EncodeToPNG();
                        System.IO.File.WriteAllBytes(path, bytes);
                        AssetDatabase.Refresh();
                        Log("Processed Texture Saved", EditorLogLevel.Info);
                    }
                }
            }
        }

        private void ProcessImage()
        {
            if (texTarget == null || texRef == null)
            {
                Log("No Target Texture Selected", EditorLogLevel.Warning);
                return;
            }

            EnsureTextureIsReadable(texTarget);
            EnsureTextureIsReadable(texRef);

            SKIDImage skidImage = ConvertTexture(texTarget);
            ImageProcessType cmd = GenerateToCommandOption(processType, constantValue,texRef);

            ImageProcessInputOption order = new(cmd,1.1f);

            var cmd2 = new ImageProcessInput<ImageProcessInputOption>(skidImage, config: order);
            var convertedImage = Generator.Process(ImageProcessCommand.NewProcessImage(cmd2), GPUAccelerator);

            texResult = ConvertToTexture(convertedImage);
            Log("Image Processed", EditorLogLevel.Info);
        }

        private void EnsureTextureIsReadable(Texture2D texture)
        {
            if (!texture.isReadable)
            {
                LuticaLabFolderManager.AssetSetReadWrite(texture);
            }
        }
    }
}
#endif