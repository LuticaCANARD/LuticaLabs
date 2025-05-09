#if UNITY_EDITOR
using LuticaSKID;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static LuticaSKID.StructTypes;
using static LuticaLab.SKIDUnityConverter;
using static LuticaLab.LuticaLabLogger;
using System.IO;


namespace LuticaLab.MeshMetro
{
    /// <summary>
    /// Normal Height Map for MeshMetro.
    /// </summary>
    public class NormalHeightMap : MeshMetroEditorContent
    {
        Texture2D texTarget;
        Texture2D normalMapPreviewTex;
        Mesh sourceMesh;

        public override void ShowOnMetro()
        {

            sourceMesh = (Mesh)EditorGUILayout.ObjectField("Source Mesh", sourceMesh, typeof(Mesh), false);
            texTarget = (Texture2D)EditorGUILayout.ObjectField("Target Texture", texTarget, typeof(Texture2D), false);
            if (sourceMesh != null)
            {
                if (GUILayout.Button("Generate Normal Map"))
                {
                    // Generate Normal Map
                    if (texTarget != null)
                    {
                        if (!texTarget.isReadable)
                            LuticaLabFolderManager.AssetSetReadWrite(texTarget);

                        SKIDImage skidImage = ConvertTexture(texTarget);

                        ImageProcessCommand cmd = ImageProcessCommand.NewGenerateNormalMapFromUV(
                            new ImageProcessInput<NormalModule.UVNormalMapMakeConfig>(
                            skidImage,
                            new NormalModule.UVNormalMapMakeConfig(
                                ConvertToSKID(sourceMesh.uv),
                                ConvertToSKID(sourceMesh.normals),
                                sourceMesh.triangles
                                )
                            ));
                        var convertedImage = Generator.Process(cmd, GPUAccelerator);
                        normalMapPreviewTex = ConvertToTexture(convertedImage);
                        Log("Normal Map Generated", EditorLogLevel.Info);
                    }
                    else
                    {
                        Log("No Target Texture Selected", EditorLogLevel.Warning);
                    }
                }
                if (normalMapPreviewTex != null)
                {
                    EditorGUILayout.LabelField("Normal Map Preview", EditorStyles.boldLabel);
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(300), GUILayout.Height(300));
                    GUILayout.Label(normalMapPreviewTex, GUILayout.Width(256), GUILayout.Height(256));
                    if (GUILayout.Button("Save..."))
                    {
                        string path = EditorUtility.SaveFilePanel("Save Normal Map", "", "NormalMap.png", "png");
                        if (!string.IsNullOrEmpty(path))
                        {
                            byte[] bytes = normalMapPreviewTex.EncodeToPNG();
                            File.WriteAllBytes(path, bytes);
                            AssetDatabase.Refresh();
                            Log("Normal map saved to: " + path);
                        }
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
        
        }
    }
}
#endif