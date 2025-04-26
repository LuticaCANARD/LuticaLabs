#if UNITY_EDITOR
using LuticaSKID;
using System.Collections;
using System.Collections.Generic;
using static LuticaSKID.StructTypes;
using static LuticaLab.SKIDUnityConverter;
using static LuticaLab.LuticaLabLogger;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;


namespace LuticaLab.MeshMetro
{
    /// <summary>
    /// Color Generator for MeshMetro.
    /// </summary>

    public class ColorCalculator : MeshMetroEditorContent
    {
        bool calculateNormalMapMode;
        const float MIN_FACTOR = 0f;
        const float MAX_FACTOR = 5f;
        
        Texture2D texTarget;
        Texture2D referenceTexture;
        Texture2D generatedTexture;
        int iterationCount;
        int clusterCount;
        float addFactor;

        public override void ShowOnMetro()
        {
            CreateTextureField("Source Texture", ref texTarget);
            CreateTextureField("Reference Texture", ref referenceTexture);
            calculateNormalMapMode = EditorGUILayout.Foldout(calculateNormalMapMode, "Options");
            if (calculateNormalMapMode)
            {
                EditorGUILayout.LabelField("Normal Map Mode", EditorStyles.boldLabel);
                clusterCount = EditorGUILayout.IntField("Cluster Count", clusterCount);
                iterationCount = EditorGUILayout.IntField("Iteration Count", iterationCount);
                addFactor = EditorGUILayout.Slider("Add Factor", addFactor, 0f, 1f);
            }
            if (GUILayout.Button("Calculate"))
            {
                generatedTexture = null;
                if (texTarget != null && referenceTexture != null)
                {
                    DateTime startTime = DateTime.Now;
                    if (!texTarget.isReadable)
                        LuticaLabFolderManager.AssetSetReadWrite(texTarget);
                    if (!referenceTexture.isReadable)
                        LuticaLabFolderManager.AssetSetReadWrite(referenceTexture);
                    if(addFactor > 1f || addFactor < 0f)
                    {
                        addFactor = Mathf.Clamp(addFactor, 0f, 1f);
                        Log("Sampling Factor must be between 0 and 1", EditorLogLevel.Warning);
                    }
                    SKIDImage skidImage = ConvertTexture(texTarget);
                    SKIDImage referenceImage = ConvertTexture(referenceTexture);
                    var debugTime = DateTime.Now - startTime;
                    startTime = DateTime.Now;
                    Log($"ConvertTime :{debugTime.TotalMilliseconds} ms");
                    ImageProcessCommand cmd = ImageProcessCommand.NewGenerateAvgTexture(
                        new ImageProcessInput<ColorMath.ColorMoodOption>(
                            skidImage,
                            new ColorMath.ColorMoodOption(
                                referenceImage,
                                iterationCount,
                                clusterCount,
                                addFactor
                            )
                        ));
                    var convertedImage = Generator.Process(cmd);
                    debugTime = DateTime.Now - startTime;
                    Log($"Color Mood Calculation Time: {debugTime.TotalMilliseconds} ms", EditorLogLevel.Info);
                    generatedTexture = ConvertToTexture(convertedImage);

                }

            }
            if (generatedTexture)
            {
                EditorGUILayout.LabelField("Generated Texture", EditorStyles.boldLabel);
                GUILayout.Label(generatedTexture,
                    GUILayout.Width(position.width),
                    GUILayout.Height(position.width * generatedTexture.height / generatedTexture.width));
                if (GUILayout.Button("Save Generated Texture"))
                {
                    string path = EditorUtility.SaveFilePanel("Save Generated Texture",
                        AssetDatabase.GetAssetPath(texTarget),
                        $"GeneratedTexture.png", "png");
                    if (!string.IsNullOrEmpty(path))
                    {
                        byte[] bytes = generatedTexture.EncodeToPNG();
                        File.WriteAllBytes(path, bytes);
                        AssetDatabase.Refresh();
                        Log("Generated texture saved to: " + path);
                    }
                }
            }
        }

    }
}
#endif