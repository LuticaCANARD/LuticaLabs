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
    public class ColorGroupAnalysisWindow : MeshMetroEditorContent
    {
        public override void ShowOnMetro()
        {
            EditorGUILayout.LabelField("Color Group Analysis", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("This feature is not implemented yet.", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Please check back later.", EditorStyles.boldLabel);
        }
    }
}
#endif