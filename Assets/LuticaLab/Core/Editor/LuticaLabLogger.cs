#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LuticaLab
{
    public static class LuticaLabLogger
    {
        public enum EditorLogLevel
        {
            Info,
            Warning,
            Error
        }
        const string LUTICALAB_LOGGER_PREFIX = " LuticaLab - ";
        public static void EditorLogger(string module, string message, EditorLogLevel level)
        {
            switch (level)
            {
                case EditorLogLevel.Info:
                    Debug.Log($"[{LUTICALAB_LOGGER_PREFIX}{module} ] {message}");
                    break;
                case EditorLogLevel.Warning:
                    Debug.LogWarning($"[{LUTICALAB_LOGGER_PREFIX}{module} ] {message}");
                    break;
                case EditorLogLevel.Error:
                    Debug.LogError($"[{LUTICALAB_LOGGER_PREFIX}{module} ] {message}");
                    break;
            }
        }
    }
}
#endif