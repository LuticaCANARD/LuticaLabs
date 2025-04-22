#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace LuticaLab.MeshMetro
{
    public class SKIDFlow
    {
        public SKIDFlow(int clusterCount, int iterationCount, float addFactor)
        {
            this.clusterCount = clusterCount;
            this.iterationCount = iterationCount;
            this.addFactor = addFactor;
        }
        public int clusterCount;
        public int iterationCount;
        public float addFactor;
    }
    /// <summary>
    /// Image Viewer for MeshMetro.
    /// </summary>
    [ExecuteInEditMode]
    public class SKIDFlowViewer : EditorWindow
    {

    }
}
#endif