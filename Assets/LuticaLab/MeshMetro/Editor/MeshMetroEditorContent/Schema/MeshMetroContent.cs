#if UNITY_EDITOR
using LuticaSKID;
using System;
using UnityEditor;
using UnityEngine;
using static LuticaLab.LuticaLabLogger;

namespace LuticaLab.MeshMetro
{
    /// <summary>
    /// MeshMetroEditorContent
    /// </summary>
    public abstract class MeshMetroEditorContent : EditorWindow
    {
        private readonly Lazy<LuticaSKIDAPI> _generator = new();
        protected LuticaSKIDAPI Generator {
            get => _generator.Value;
        }
        static protected void Log(string message, EditorLogLevel level = EditorLogLevel.Info)
        {
            EditorLogger("SKID/MeshMetro", message, level);
        }
        protected Vector2 scrollPosition;
        protected void CreateField<T>(string label, ref T value) where T : UnityEngine.Object
        {
            GUILayout.Label(label);
            value = (T)EditorGUILayout.ObjectField(value, typeof(T), false);
        }
        protected void CreateTextureField(string label, ref Texture2D value)
        {
            value = (Texture2D)EditorGUILayout.ObjectField(label,value, typeof(Texture2D), false);
        }
        protected void CreateTextureDisplayField(Texture2D label,params GUILayoutOption[] options)
        {
            GUILayout.Label(label, options);
        }
        public abstract void ShowOnMetro();
    }
    public class MeshMetroOptionArgument<T>
    {
        public T Option { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MeshMetroOptionArgument(T option)
        {
            Option = option;
            Name = option.GetType().Name;
            Description = option.ToString();
        }
    }
}
#endif