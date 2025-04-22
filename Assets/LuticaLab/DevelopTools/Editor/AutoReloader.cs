#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class AutoReloader
{
    /// <summary>
    /// 이 클래스는 Unity 에디터가 시작될 때 자동으로 AssetDatabase를 새로 고침합니다.
    /// F#개발등에 사용합니다.
    /// </summary>
    static AutoReloader()
    {
        //AssetDatabase.Refresh();
    }
}
#endif