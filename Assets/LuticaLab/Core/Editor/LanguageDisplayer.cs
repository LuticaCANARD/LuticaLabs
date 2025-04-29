#if UNITY_EDITOR
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization.Editor;
using UnityEngine;

namespace LuticaLab
{
    public enum LuticaLabSupportLanguage
    {
        English,
        Korean,
        Japanese
    }
    [Serializable]
    public class Langattributs
    {
        public string key;
        public string translated_text;
    }
    public class LanguageDisplayer:ScriptableObject
    {
        private static readonly Lazy<LanguageDisplayer> _languageDisplayer = new(() =>
        {
            LanguageDisplayer instance = CreateInstance<LanguageDisplayer>();
            instance.name = "LanguageDisplayer";
            return instance;
        });
        public static LanguageDisplayer Instance => _languageDisplayer.Value;
        // ID-Value...
        Dictionary<string, string> _languageDictionary = new();
        LuticaLabSupportLanguage _lang = LuticaLabSupportLanguage.English;
        private void OnEnable()
        {
            NowLanguage = GenerateToLuticaLabSupportLanguage(Application.systemLanguage);
            LoadLanguageDict(NowLanguage);
        }
        public LuticaLabSupportLanguage NowLanguage
        {
            get => _lang;
            set
            {
                if (_lang != value)
                {
                    _lang = value;
                    _languageDictionary.Clear();
                    LoadLanguageDict(_lang);


                }
            }
        }
        void LoadLanguageDict(LuticaLabSupportLanguage lang)
        {
            var jsonload = AssetDatabase.LoadAssetAtPath($"Assets/LuticaLab/Languages/{lang}.json", typeof(TextAsset)) as TextAsset;
            if (jsonload != null)
            {

                if (jsonload.text != null)
                {
                    var json = jsonload.text;
                    var dict = JObject.Parse(json)["data"] as JObject;
                    foreach (var item in dict)
                    {
                        _languageDictionary[item.Key.ToString()] = item.Value.ToString();
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load language file for {lang}");
                }
            }
            else
            {
                Debug.LogError($"Failed to load language bundle for {lang}");
            }
        }
        public bool IsSupportedLanguage(SystemLanguage lang)
        {
            return lang == SystemLanguage.English 
                || lang == SystemLanguage.Korean 
                || lang == SystemLanguage.Japanese;
        }
        static public LuticaLabSupportLanguage GenerateToLuticaLabSupportLanguage(SystemLanguage lang,bool error = false)
        {
            return lang switch
            {
                SystemLanguage.English => LuticaLabSupportLanguage.English,
                SystemLanguage.Korean => LuticaLabSupportLanguage.Korean,
                SystemLanguage.Japanese => LuticaLabSupportLanguage.Japanese,
                _ => error ? throw new ArgumentException($"{lang} is Unsupported Language!") : LuticaLabSupportLanguage.English
            };
        }
        public string GetTranslatedLanguage(string key) =>
            _languageDictionary.TryGetValue(key, out var value) ? value : key;
    }
}
#endif