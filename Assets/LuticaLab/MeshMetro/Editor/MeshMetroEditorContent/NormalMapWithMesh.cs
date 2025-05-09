#if UNITY_EDITOR
using LuticaSKID;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using static LuticaSKID.StructTypes;
using static LuticaLab.SKIDUnityConverter;
using static LuticaLab.LuticaLabLogger;

namespace LuticaLab.MeshMetro
{
    /// <summary>
    /// Color Generator for MeshMetro.
    /// </summary>

    public class NormalMapWithMesh : MeshMetroEditorContent
    {
        const float MIN_FACTOR = 0f;
        const float MAX_FACTOR = 5f;
        Texture2D texTarget;
        Texture2D normalMapPreviewTex;
        float xFactor;
        float yFactor;

        public override void ShowOnMetro()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label( LanguageDisplayer.Instance.GetTranslatedLanguage("Normal Map Generator"), EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            texTarget = (Texture2D)EditorGUILayout.ObjectField(LanguageDisplayer.Instance.GetTranslatedLanguage("Source Image"), texTarget, typeof(Texture2D), false);
            xFactor = EditorGUILayout.Slider(string.Format(LanguageDisplayer.Instance.GetTranslatedLanguage("{0} Factor"),"X"), xFactor, MIN_FACTOR, MAX_FACTOR);
            yFactor = EditorGUILayout.Slider(string.Format(LanguageDisplayer.Instance.GetTranslatedLanguage("{0} Factor"), "Y"), yFactor, MIN_FACTOR, MAX_FACTOR);
            if (GUILayout.Button(LanguageDisplayer.Instance.GetTranslatedLanguage("Generate Normal Map")))
            {
                GenerateNormalMap();
            }
            if (normalMapPreviewTex != null)
            {
                if (GUILayout.Button(LanguageDisplayer.Instance.GetTranslatedLanguage("Save Normal Map")))
                {
                    string path = EditorUtility.SaveFilePanel(
                        LanguageDisplayer.Instance.GetTranslatedLanguage("Save Normal Map"),
                        AssetDatabase.GetAssetPath(texTarget),
                        $"NormalMap.png", "png");
                    if (!string.IsNullOrEmpty(path))
                    {
                        byte[] bytes = normalMapPreviewTex.EncodeToPNG();
                        File.WriteAllBytes(path, bytes);
                        AssetDatabase.Refresh();
                        Log("Normal map saved to: " + path);
                    }
                }
                GUILayout.Label(LanguageDisplayer.Instance.GetTranslatedLanguage("Normal Map Preview:"), EditorStyles.boldLabel);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,true, true);
                GUILayout.Label(normalMapPreviewTex,
                    GUILayout.Width(normalMapPreviewTex.width),
                    GUILayout.Height(normalMapPreviewTex.height));

                EditorGUILayout.EndScrollView();
            }
        }
        void GenerateNormalMap()
        {
            normalMapPreviewTex = null;
            if (texTarget == null)
            {
                Log(LanguageDisplayer.Instance.GetTranslatedLanguage("Please select a source image."), EditorLogLevel.Warning);
                return;
            }
            if (!texTarget.isReadable) LuticaLabFolderManager.AssetSetReadWrite(texTarget);
            ImageProcessCommand cmd = ImageProcessCommand.NewGenerateNormalMap(
                new ImageProcessInput<NormalModule.NormalMapConfig>(ConvertTexture(texTarget),
                new NormalModule.NormalMapConfig(
                xFactor, yFactor
                )
            ));
            SKIDImage processedImage = Generator.Process(cmd, GPUAccelerator);
            normalMapPreviewTex = ConvertToTexture(processedImage);
        }

    }
}
#endif