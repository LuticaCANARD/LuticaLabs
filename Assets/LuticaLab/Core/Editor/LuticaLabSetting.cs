using LuticaLab;
using UnityEditor;
using UnityEngine;

namespace Assets.LuticaLab.Core.Editor
{
    public class LuticaLabSetting : EditorWindow
    {
        [MenuItem("LuticaLab/Setting", false, 0)]
        public static void Init()
        {
            LuticaLabSetting window = (LuticaLabSetting)GetWindow(typeof(LuticaLabSetting));
            window.titleContent = new GUIContent("Lutica Lab Setting");
            window.Show();
        }
        private void OnGUI()
        {
            GUILayout.Label("Lutica Lab Setting", EditorStyles.boldLabel);
            
            LanguageDisplayer.Instance.NowLanguage = (LuticaLabSupportLanguage)EditorGUILayout.EnumPopup("Language", LanguageDisplayer.Instance.NowLanguage);
        }
    }
  
}