#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static LuticaLab.SKIDUnityConverter;

namespace LuticaLab.MeshMetro
{
    /// <summary>
    /// Debug Content for MeshMetroEditor
    /// </summary>
    public class DebugContentPMP : EditorWindow
    {
        Mesh targetMesh;
        Texture2D targetTexture;
        Texture2D displayTxt;
        bool[] uvChanelBit = new bool[8];
        [MenuItem("LuticaLab/Debug/DebugContentPMP")]
        public static void ShowWindow()
        {
            GetWindow<DebugContentPMP>("DebugContentPMP");
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Debug Content for MeshMetroEditor", EditorStyles.boldLabel);
            targetMesh = (Mesh)EditorGUILayout.ObjectField("Target Mesh", targetMesh, typeof(Mesh), true);
            targetTexture = (Texture2D)EditorGUILayout.ObjectField("Target Texture", targetTexture, typeof(Texture2D), true);
            for(int i = 0; i < 8; i++)
            {
                uvChanelBit[i] = EditorGUILayout.Toggle($"UV Channel {i}", uvChanelBit[i]);
            }
            if (GUILayout.Button("Test"))
            {
                if(!targetMesh.isReadable)
                {
                    LuticaLabFolderManager.AssetSetReadWrite(targetMesh);
                }
                if(!targetTexture.isReadable)
                {
                    LuticaLabFolderManager.AssetSetReadWrite(targetTexture);
                }

                displayTxt = ConvertToTexture(GenerateMaskMap(targetMesh, ConvertTexture(targetTexture),MakeUVChanelBit(
                    uv0: uvChanelBit[0],
                    uv1: uvChanelBit[1],
                    uv2: uvChanelBit[2],
                    uv3: uvChanelBit[3],
                    uv4: uvChanelBit[4],
                    uv5: uvChanelBit[5],
                    uv6: uvChanelBit[6],
                    uv7: uvChanelBit[7]
                    ))); 
            }
            if(displayTxt != null)
            {
                EditorGUILayout.LabelField("Display Texture", EditorStyles.boldLabel);
                GUILayout.Label(displayTxt);
            }
        }


    }
}


#endif