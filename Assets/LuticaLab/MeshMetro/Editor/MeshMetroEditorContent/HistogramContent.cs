#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LuticaSKID;
using static LuticaSKID.StructTypes;
using static LuticaLab.SKIDUnityConverter;
using static LuticaLab.LuticaLabLogger;
using LuticaSKID.Models;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;


namespace LuticaLab.MeshMetro
{
    /// <summary>
    /// Histogram Content
    /// </summary>
    public class HistogramContent : MeshMetroEditorContent
    {
        const int DEFAULT_IMAGE_LEVEL = 256;
        Texture2D targetTexture;
        AnalyzeResultTypes.HistogramResult result;
        Mesh targetModel;
        int histogramWidth;
        int before_width;
        Vector2 histogramAnalScroll;

        bool[] uvChanelBit = new bool[8];

        Dictionary<int,int> histogramData = new Dictionary<int, int>();
        public override void ShowOnMetro()
        {
            EditorGUILayout.LabelField("Histogram", EditorStyles.boldLabel);

            CreateField("Target Texture", ref targetTexture);
            histogramWidth = EditorGUILayout.IntField("Histogram Width", histogramWidth);
            targetModel = (Mesh)EditorGUILayout.ObjectField("Model...", targetModel, typeof(Mesh), true);
            if (targetModel != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    uvChanelBit[i] = EditorGUILayout.Toggle($"UV Channel {i}", uvChanelBit[i]);
                }
            }
            if (GUILayout.Button("Open Histogram"))
            {
                Process();
            }
            if(result != null)
            {
                EditorGUILayout.LabelField("Histogram Result", EditorStyles.boldLabel);
                histogramAnalScroll = EditorGUILayout.BeginScrollView(histogramAnalScroll);
                foreach(var keys in histogramData.Keys)
                {
                    EditorGUILayout.LabelField($"Color {keys} (R:{keys/ before_width / before_width}/G:{keys/ before_width % before_width}/B:{keys% before_width}) : {histogramData[keys]}");
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField("No Histogram Result", EditorStyles.boldLabel);
            }
        }

        void Process()
        {
            var mask = targetModel != null
                ? GenerateMaskMap(targetModel, ConvertTexture(targetTexture), MakeUVChanelBit(
                    uv0: uvChanelBit[0],
                    uv1: uvChanelBit[1],
                    uv2: uvChanelBit[2],
                    uv3: uvChanelBit[3],
                    uv4: uvChanelBit[4],
                    uv5: uvChanelBit[5],
                    uv6: uvChanelBit[6],
                    uv7: uvChanelBit[7]
                )) : FSharpOption<SKIDImage>.None; // Use FSharpOption.None to represent absence of a value
            if(histogramWidth == 0)
            {
                histogramWidth = DEFAULT_IMAGE_LEVEL;
            }
            var histogramAnalyzeOption = new HistogramProcessor.histogramAnalyzeOption(histogramWidth, false, DEFAULT_IMAGE_LEVEL, mask);

            if (targetTexture == null)
            {
                Log("Target texture is null.", EditorLogLevel.Error);
                return;
            }

            if (!targetTexture.isReadable)
            {
                LuticaLabFolderManager.AssetSetReadWrite(targetTexture);
            }

            histogramData.Clear();
            var convImg = ConvertTexture(targetTexture);
            ImageAnalyzeCommand cmd = ImageAnalyzeCommand.NewAnalyzeHistogram(
                new ImageProcessInput<HistogramProcessor.histogramAnalyzeOption>(
                    convImg,
                    histogramAnalyzeOption
                )
            );

            var histogramedImg = Generator.AnalyzingColorImage(cmd, GPUAccelerator);
            if (histogramedImg != null)
            {
                before_width = histogramWidth;
                result = (AnalyzeResultTypes.HistogramResult)histogramedImg;
                for (int i = 0; i < result.Item.result.histogram.Length; i++)
                {
                    if (!histogramData.ContainsKey(i) && result.Item.result.histogram[i] > 0)
                        histogramData.Add(i, result.Item.result.histogram[i]);
                }
            }
            else
            {
                Log("Failed to generate histogram image.", EditorLogLevel.Error);
            }
        }
    }
}
#endif