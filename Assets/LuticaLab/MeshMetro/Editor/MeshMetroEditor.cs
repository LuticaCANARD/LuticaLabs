#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LuticaSKID;
using static LuticaSKID.StructTypes;
using static LuticaLab.SKIDUnityConverter;
using static LuticaLab.LuticaLabLogger;
using System;
using System.IO;
using LuticaLab.MeshMetro;

namespace LuticaLab
{
    /// <summary>
    /// 전 방위적인 Mesh-Texture 변환/통합 툴이자 비주엂적 편의기능을 갖춘 Mesh-Texture 변환기.
    /// </summary>
    public class MeshMetroEditor : EditorWindow
    {
        // 1. Mesh를 가져와서 Normal Map을 생성.
        // 2. Normal Map을 가져와서 Mesh에 적용.
        // 3. 뚜따시 두 텍스쳐간 색조 통합
        // 4. Normal Map을 가져와서 Mesh에 적용.
        // 5. HeighrMap등도 추가할 수 있도록.
        // 6. 두 머테리얼간의 쉐이더 역시 통합가능하게 작성.


        // Future : HLSL로 텍스쳐 변형기능을 제공해보자.
        // 가능하면 베이킹도 나쁘지 않을듯 함.
        public enum ImageProcessSelect
        {
            NormalHeightMap,
            NormalMapWithMesh,
            ColorCalculate,
            ImageProcess,
            ImageSynthesis,
        }
        Dictionary<ImageProcessSelect, MeshMetroEditorContent> content;
        static void Log(string message, EditorLogLevel level = EditorLogLevel.Info)
        {
            EditorLogger("SKID/MeshMetro", message, level);
        }
        [MenuItem("LuticaLab/MeshMetro")]
        public static void ShowWindow()
        {
            GetWindow<MeshMetroEditor>("MeshMetro");
        }
        ImageProcessSelect select;
        public void OnGUI()
        {
            // TITLE...
            // 엄청 긴 타이틀을 넣어도 상관없음.
            // 타이틀은 EditorWindow의 타이틀과는 다름.
            // EditorWindow의 타이틀은 상단에 보이는 타이틀임.
            EditorGUILayout.Space(15);
            GUIStyle headStyle = new()
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState()
                {
                    textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black
                }
            };
            EditorGUILayout.LabelField("Mesh Metro", headStyle);
            EditorGUILayout.Space(15);
            select = (ImageProcessSelect)EditorGUILayout.EnumPopup(select);
            MeshMetroEditorContent meshMetroEditorContent = GetContent(select);
            if (meshMetroEditorContent != null)
            {
                meshMetroEditorContent.ShowOnMetro();
            }
            else
            {
                UnknownPage();
            }
        }

        MeshMetroEditorContent GetContent(ImageProcessSelect select)
        {
            content ??= new Dictionary<ImageProcessSelect, MeshMetroEditorContent>();

            if (!content.ContainsKey(select))
            {
                content[select] = select switch
                {
                    ImageProcessSelect.ColorCalculate => CreateInstance<ColorCalculator>(),
                    ImageProcessSelect.NormalMapWithMesh => CreateInstance<NormalMapWithMesh>(),
                    ImageProcessSelect.NormalHeightMap => CreateInstance<NormalHeightMap>(),
                    ImageProcessSelect.ImageProcess => CreateInstance<ImageProcessor>(),
                    ImageProcessSelect.ImageSynthesis => CreateInstance<ImageSynthesisWindow>(),
                    _ => null
                };
            }
            return content[select];
        }
        static void UnknownPage() => Log("Unknown Command... Please Report To presan100@gmail.com", EditorLogLevel.Error);

    }
}

#endif
