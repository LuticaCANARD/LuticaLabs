#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LuticaLab.MeshMetro
{
    /// <summary>
    /// Image Viewer for MeshMetro.
    /// </summary>
    [ExecuteInEditMode]
    public class ImageViewer : EditorWindow
    {

        public Texture2D image;
        public void OnEnable()
        {
            titleContent = new GUIContent("Image Viewer");
        }

        public void OnGUI()
        {
            if (image != null)
            {
                Rect rect = new (0, 0, position.width, position.height);
                GUI.DrawTexture(rect, image, ScaleMode.ScaleToFit);
            }
            else
            {
                EditorGUILayout.LabelField("No image to display.");
            }
        }

        public void SetImage(Texture2D texture)
        {
            image = texture;
            Repaint();
        }

    }
}
#endif