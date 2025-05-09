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
using LuticaSKID.Models;
using System.Threading.Tasks;


namespace LuticaLab.MeshMetro
{
    public class ColorGroupAnalysisWindow : MeshMetroEditorContent
    {

        Texture2D analysisTarget;
        Texture2D c_analysisTarget;

        AnalyzeResultTypes.ColorGroupingResult analyzeResult;
        int maxK;
        int iterationCount;
        bool doNotPickWhite;
        Task analyzeTask;
        public override void ShowOnMetro()
        {

            CreateTextureField("Source Texture", ref analysisTarget);
            maxK = EditorGUILayout.IntField("Max K", maxK);
            iterationCount = EditorGUILayout.IntField("Iteration Count", iterationCount);
            doNotPickWhite = EditorGUILayout.Toggle("Do Not Pick White", doNotPickWhite);
           
            if (GUILayout.Button("Analyze Color Group"))
            {
                StartAnalylze();
            }
            if (analyzeTask != null && !analyzeTask.IsCompleted)
            {
                EditorGUILayout.LabelField("Analyzing...", EditorStyles.boldLabel);
            }
            if(analyzeResult != null)
            {
                EditorGUILayout.LabelField("Analyze Result", EditorStyles.boldLabel);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                foreach (var result in analyzeResult.Item.result.colorResult)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Color Group", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("R:" + result.colorElement.r.ToString());
                    EditorGUILayout.LabelField("G:" + result.colorElement.g.ToString());
                    EditorGUILayout.LabelField("B:" + result.colorElement.b.ToString());
                    EditorGUILayout.LabelField("A:" + result.colorElement.a.ToString());
                    EditorGUILayout.ColorField(ConvertToUnity(result.colorElement));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("Weight:" + result.weight.ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
        }

        public void StartAnalylze()
        {
            if(analyzeTask != null && !analyzeTask.IsCompleted)
            {
                Log("Already analyzing. Please wait.", EditorLogLevel.Warning);
                return;
            }
            if (analysisTarget == null)
            {
                Log("Please select a texture to analyze.", EditorLogLevel.Warning);
                return;
            }
            if (!analysisTarget.isReadable)
            {
                LuticaLabFolderManager.AssetSetReadWrite(analysisTarget);
            }
            if(maxK <= 4)
            {
                maxK = 4;
            }
            if (iterationCount <= 0)
            {
                iterationCount = 1;
            }
            analyzeResult = null;
            // Fix: Ensure the correct generic type is used for ImageProcessInput
            var kmeansSetting = new ColorGroupingModel.KmeansSetting(maxK,iterationCount, doNotPickWhite); // Assuming a default instance

            
            var inputWithSetting = new ImageProcessInput<ColorGroupingModel.KmeansSetting>(ConvertTexture(analysisTarget), kmeansSetting);
            analyzeTask = Task.Run(() =>
            {
                try
                {
                    analyzeResult = (AnalyzeResultTypes.ColorGroupingResult)Generator.AnalyzingColorImage(ImageAnalyzeCommand.NewAnalyzeColorGroup(inputWithSetting), GPUAccelerator);
                }
                catch (Exception e)
                {
                    Log("Analyze failed: " + e.Message, EditorLogLevel.Error);
                }
            }).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Log("Analyze failed: " + t.Exception, EditorLogLevel.Error);
                }
                else
                {
                    Log("Analyze completed successfully.", EditorLogLevel.Info);
                    Debug.Log("Analyze Result : " + analyzeResult.Item.result);
                }
            });

            
        }


    }
}
#endif