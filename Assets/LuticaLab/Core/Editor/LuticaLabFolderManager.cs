#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace LuticaLab
{
    public static class LuticaLabFolderManager
    {
        static void Log(string message, LuticaLabLogger.EditorLogLevel level = LuticaLabLogger.EditorLogLevel.Info)
        {
            LuticaLabLogger.EditorLogger("LuticaLabFolderManager", message, level);
        }
        public static void AssetSetReadWrite<T>(T target) where T : Object
        {
            string assetPath = AssetDatabase.GetAssetPath(target);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
                Log("File Read/Write Setted !"+ assetPath);
            }
        }

    }
}
#endif