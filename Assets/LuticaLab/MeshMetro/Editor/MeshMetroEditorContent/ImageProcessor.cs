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
        public enum ImageProcessCommandOrder
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            AverageImage,
            BlondColor,
            ReplaceTexture,
            ColorDifference,
            Invert,
            Brightness,
            Contrast,
            Grayscale,

        }

        private Texture2D texTarget;
        private Texture2D texRef;
        private Texture2D texResult;
        private ImageProcessCommandOrder processType;
        private float constantValue;
        private Vector2 ImageScrollVector;

        private ImageProcessType GenerateToCommandOption(ImageProcessCommandOrder cmd)
        {
            return cmd switch
            {
                ImageProcessCommandOrder.Add => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.Add,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.Subtract => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.Subtract,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.Multiply => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.Multiply,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.Divide => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.Divide,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.AverageImage => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.Average,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.BlondColor => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.ColorBlend,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.ColorDifference => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.ColorDifference,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.ReplaceTexture => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.TextureReplace,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.Invert => ImageProcessType.NewSingleImageProcess(
                    SingleImageProcessType.Invert
                ),
                //ImageProcessCommandOrder.Brightness => ImageProcessType.NewSingleImageProcess(
                //    SingleImageProcessType.Brightness,
                //    new SingleImageProcessOption(constantValue)
                //),
                _ => throw new System.NotImplementedException("Unknown Image Process Command"),
            };
        }

        public override void ShowOnMetro()
        {
            EditorGUILayout.LabelField("Image Process", EditorStyles.boldLabel);
            texTarget = (Texture2D)EditorGUILayout.ObjectField("Target Texture", texTarget, typeof(Texture2D), false);
            texRef = (Texture2D)EditorGUILayout.ObjectField("Reference Texture", texRef, typeof(Texture2D), false);
            processType = (ImageProcessCommandOrder)EditorGUILayout.EnumPopup("Process Type", processType);
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
            ImageProcessType cmd = GenerateToCommandOption(processType);
            ImageProcessInputOption order = new(cmd,1.1f);

            var cmd2 = new ImageProcessInput<ImageProcessInputOption>(skidImage, config: order);
            var convertedImage = generator.Process(ImageProcessCommand.NewProcessImage(cmd2));

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