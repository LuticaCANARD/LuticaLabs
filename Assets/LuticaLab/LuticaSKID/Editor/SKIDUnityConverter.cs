#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using static LuticaLab.MeshMetro.ImageProcessor;
using static LuticaSKID.Models.TextureImageProcessing;
using static LuticaSKID.StructTypes;

namespace LuticaLab
{
    public enum ImageProcessCommandOrder
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        AverageImage,
        BlondColor,
        ReplaceTexture,
        ColorDifference,
        Invert,
        //Brightness,
        //Contrast,
        //Grayscale,
        Median,

    }
   
    public static class SKIDUnityConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SKIDImage ConvertTexture(Texture2D tex) => new(ConvertToSKID(tex.GetPixels32()), tex.width, tex.height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Texture2D ConvertToTexture(SKIDImage skidImage)
        {
            Texture2D tex = new(skidImage.width, skidImage.height, TextureFormat.RGBA32, false);
            tex.SetPixels32(ConvertToUnity(skidImage.pixels));
            tex.Apply();
            return tex;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SKIDColor ConvertToSKID(Color32 colors) => new(colors.r / 255f, colors.g / 255f, colors.b / 255f, colors.a / 255f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 ConvertToUnity(SKIDColor color) => new((byte)(Mathf.Clamp01(color.r) * 255f), (byte)(Mathf.Clamp01(color.g) * 255f), (byte)(Mathf.Clamp01(color.b) * 255f), (byte)(Mathf.Clamp01(color.a) * 255f));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SKIDVector2 ConvertToSKID(Vector2 vector) => new SKIDVector2(vector.x, vector.y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ConvertToUnity(SKIDVector2 vector) => new Vector2(vector.x, vector.y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SKIDVector3 ConvertToSKID(Vector3 vector) => new SKIDVector3(vector.x, vector.y, vector.z);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ConvertToUnity(SKIDVector3 vector) => new Vector3(vector.x, vector.y, vector.z);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SKIDColor[] ConvertToSKID(Color32[] colors)
        {
            SKIDColor[] result = new SKIDColor[colors.Length];
            Parallel.For(0, colors.Length, i =>
            {
                result[i] = ConvertToSKID(colors[i]);
            });
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32[] ConvertToUnity(SKIDColor[] colors)
        {
            Color32[] result = new Color32[colors.Length];
            Parallel.For(0, colors.Length, i =>
            {
                result[i] = ConvertToUnity(colors[i]);
            });
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SKIDVector2[] ConvertToSKID(Vector2[] vectors)
        {
            SKIDVector2[] result = new SKIDVector2[vectors.Length];
            Parallel.For(0, vectors.Length, i =>
            {
                result[i] = ConvertToSKID(vectors[i]);
            });
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2[] ConvertToUnity(SKIDVector2[] vectors)
        {
            Vector2[] result = new Vector2[vectors.Length];
            Parallel.For(0, vectors.Length, i => result[i] = ConvertToUnity(vectors[i]));
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SKIDVector2[] ConvertToSKID(List<Vector2> vectors)
        {
            SKIDVector2[] result = new SKIDVector2[vectors.Count];
            Parallel.For(0, vectors.Count, i => result[i] = ConvertToSKID(vectors[i]));
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SKIDVector3[] ConvertToSKID(Vector3[] vectors)
        {
            SKIDVector3[] result = new SKIDVector3[vectors.Length];
            Parallel.For(0, vectors.Length, i =>
            {
                result[i] = ConvertToSKID(vectors[i]);
            });
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3[] ConvertToUnity(SKIDVector3[] vectors)
        {
            Vector3[] result = new Vector3[vectors.Length];
            Parallel.For(0, vectors.Length, i =>
            {
                result[i] = ConvertToUnity(vectors[i]);
            });
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="mask"></param>
        /// <param name="uv_chanel_bit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SKIDImage GenerateMaskMap(Mesh mesh, SKIDImage mask, byte uv_chanel_bit)
        {
            if (mesh == null || mask == null)
                throw new ArgumentNullException("Mesh or mask cannot be null.");
            List<Vector2> uvs = new();
            if ((uv_chanel_bit & (1 << 0)) == 1) uvs.AddRange(mesh.uv); // UV0
            if ((uv_chanel_bit & (1 << 1)) >= 1) uvs.AddRange(mesh.uv2); // UV1
            if ((uv_chanel_bit & (1 << 2)) >= 1) uvs.AddRange(mesh.uv3); // UV2
            if ((uv_chanel_bit & (1 << 3)) >= 1) uvs.AddRange(mesh.uv4); // UV3
            if ((uv_chanel_bit & (1 << 4)) >= 1) uvs.AddRange(mesh.uv5); // UV4
            if ((uv_chanel_bit & (1 << 5)) >= 1) uvs.AddRange(mesh.uv6); // UV5
            if ((uv_chanel_bit & (1 << 6)) >= 1) uvs.AddRange(mesh.uv7); // UV6
            if ((uv_chanel_bit & (1 << 7)) >= 1) uvs.AddRange(mesh.uv8); // UV7
            if (uvs.Count == 0)
            {
                throw new Exception("Error On GET UV..");
            }
            ConcurrentDictionary<int, bool> marked = new();
            int[] triangles = mesh.triangles; // 삼각형 인덱스
            int width = mask.width;
            int height = mask.height;
            SKIDColor[] maskPixels = new SKIDColor[width * height]; // 마스크 픽셀 초기화
            // 초기화: 전부 0 (투명)
            Parallel.For(0, maskPixels.Length, i =>
            {
                maskPixels[i] = new SKIDColor(0, 0, 0, 0);
            });

            Parallel.For(0, triangles.Length / 3, t =>
            {
                int i = t * 3;
                // 삼각형의 세 정점...
                Vector2 uv0 = uvs[triangles[i]];
                Vector2 uv1 = uvs[triangles[i + 1]];
                Vector2 uv2 = uvs[triangles[i + 2]];

                int minX = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(uv0.x, uv1.x, uv2.x) * width), 0, width - 1);
                int maxX = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(uv0.x, uv1.x, uv2.x) * width), 0, width - 1);
                int minY = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(uv0.y, uv1.y, uv2.y) * height), 0, height - 1);
                int maxY = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(uv0.y, uv1.y, uv2.y) * height), 0, height - 1);

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {

                        Vector2 point = new Vector2(x / (float)width, y / (float)height);
                        if (IsPointInTriangle(point, uv0, uv1, uv2))
                        {

                            maskPixels[y * width + x] = new SKIDColor(1, 1, 1, 1);
                        }
                    }
                }
            });
            return new SKIDImage(maskPixels, mask.width, mask.height);
        }
        // Point-in-Triangle 알고리즘
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float alpha = ((b.y - c.y) * (p.x - c.x) + (c.x - b.x) * (p.y - c.y)) / ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y));
            float beta = ((c.y - a.y) * (p.x - c.x) + (a.x - c.x) * (p.y - c.y)) / ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y));
            float gamma = 1.0f - alpha - beta;
            return alpha >= 0 && beta >= 0 && gamma >= 0;
        }

        public static byte MakeUVChanelBit(bool uv0, bool uv1 = false, bool uv2 = false, bool uv3 = false, bool uv4 = false, bool uv5 = false, bool uv6 = false, bool uv7 = false)
        {
            byte result = 0;
            if (uv0) result |= 1 << 0;
            if (uv1) result |= 1 << 1;
            if (uv2) result |= 1 << 2;
            if (uv3) result |= 1 << 3;
            if (uv4) result |= 1 << 4;
            if (uv5) result |= 1 << 5;
            if (uv6) result |= 1 << 6;
            if (uv7) result |= 1 << 7;
            return result;
        }

        public static ImageProcessType GenerateToCommandOption(ImageProcessCommandOrder cmd, float constantValue, Texture2D texRef = null)
        {
            if(texRef != null && !texRef.isReadable)
            {
                LuticaLabFolderManager.AssetSetReadWrite(texRef);
            }
            return cmd switch
            {
                ImageProcessCommandOrder.Add => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.Add,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.Subtract => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.Subtract,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.Multiply => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.Multiply,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.Divide => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.Divide,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.AverageImage => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.Average,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.BlondColor => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.ColorBlend,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.ColorDifference => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.ColorDifference,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.ReplaceTexture => ImageProcessType.NewTwoImageProcess(
                    ImageProcessTwoImage.TextureReplace,
                    new ImageProcessTwoImageOption(ConvertTexture(texRef), constantValue)
                ),
                ImageProcessCommandOrder.Invert => ImageProcessType.NewSingleImageProcess(
                    SingleImageProcessType.Invert
                ),
                ImageProcessCommandOrder.Median => ImageProcessType.NewSingleImageProcess(
                    SingleImageProcessType.Median
                ),
                _ => throw new System.NotImplementedException("Unknown Image Process Command : " + cmd.GetType().FullName),
            };
        }
        public static bool CheckNeedTwoImage(ImageProcessCommandOrder cmd)
        {
            return cmd switch
            {
                ImageProcessCommandOrder.Add => true,
                ImageProcessCommandOrder.Subtract => true,
                ImageProcessCommandOrder.Multiply => true,
                ImageProcessCommandOrder.Divide => true,
                ImageProcessCommandOrder.AverageImage => true,
                ImageProcessCommandOrder.BlondColor => true,
                ImageProcessCommandOrder.ReplaceTexture => true,
                ImageProcessCommandOrder.ColorDifference => true,
                _ => false,
            };
        }


    }
}

#endif