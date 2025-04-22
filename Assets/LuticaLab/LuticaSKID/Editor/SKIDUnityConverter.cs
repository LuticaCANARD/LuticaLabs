#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using static LuticaSKID.StructTypes;

namespace LuticaLab
{
    public static class SKIDUnityConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SKIDImage ConvertTexture(Texture2D tex) => new SKIDImage(ConvertToSKID(tex.GetPixels32()), tex.width, tex.height);
        
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
    }

}

#endif