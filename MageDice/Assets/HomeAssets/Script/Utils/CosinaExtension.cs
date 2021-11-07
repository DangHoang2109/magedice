
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Cosina.Components
{
    public static class CosinaExtension
    {
        
        public static T Random<T>(this T[] input)
        {
            return input.Length == 0
                ? default
                : input[UnityEngine.Random.Range(0, input.Length)];
        }

        public static void DownBy1(this List<int> input)
        {
            if (input is null)
                return;

            for (int i = 0; i < input.Count; i++)
            {
                --input[i];
            }
        }

        #region string

        public static string ToStringExtend<T>(this T[] input)
        {
            if (input is null)
                return "[NULL-ARRAY]";
            switch (input.Length)
            {
                case 0:
                    return "[]";
                case 1 when input[0] == null:
                    return "[null]";
                case 1:
                    return $"[{input[0]}]";
            }

            var result = input[0] == null ? "[null" : $"[{input[0]}";
            for (int i = 1; i < input.Length; ++i)
            {
                if (input[i] == null)
                {
                    result += ", null";
                }
                else
                {
                    result += $", {input[i]}";
                }
            }

            return result + "]";
        }


        public static string ToStringExtend<T>(this List<T> input)
        {
            if (input is null)
                return "[NULL-ARRAY]";
            switch (input.Count)
            {
                case 0:
                    return "[]";
                case 1 when input[0] == null:
                    return "[null]";
                case 1:
                    return $"[{input[0]}]";
            }

            var result = input[0] == null ? "[null" : $"[{input[0]}";
            for (int i = 1; i < input.Count; ++i)
            {
                if (input[i] == null)
                {
                    result += ", null";
                }
                else
                {
                    result += $", {input[i]}";
                }
            }

            return result + "]";
        }

        /// <summary>
        /// colorCode: #hexa or red green magenta white...
        /// </summary>
        public static string WrapColor(this string t, string colorCode)
        {
#if UNITY_EDITOR
            return $"<color={colorCode}>{t}</color>";
#else
            return t;
#endif
        }

        public static string WrapBold(this string t)
        {
#if UNITY_EDITOR
            return $"<b>{t}</b>";
#else
            return t;
#endif
        }

        public static string WrapItalic(this string t)
        {
#if UNITY_EDITOR
            return $"<i>{t}</i>";
#else
            return t;
#endif
        }

        /// <summary>
        /// colorCode: #hexa or red green magenta white...
        /// </summary>
        public static string WrapBoldColor(this string t, string colorCode)
        {
#if UNITY_EDITOR
            return $"<b><color={colorCode}>{t}</color></b>";
#else
            return t;
#endif
        }

        /// <summary>
        /// colorCode: #hexa or red green magenta white...
        /// </summary>
        public static string WrapItalicColor(this string t, string colorCode)
        {
#if UNITY_EDITOR
            return $"<i><color={colorCode}>{t}</color></i>";
#else
            return t;
#endif
        }

        /// <summary>
        /// colorCode: #hexa or red green magenta white...
        /// </summary>
        public static string WrapBoldItalicColor(this string t, string colorCode)
        {
#if UNITY_EDITOR
            return $"<b><i><color={colorCode}>{t}</color></i></b>";
#else
            return t;
#endif
        }

        public static string WrapBoldItalic(this string t)
        {
#if UNITY_EDITOR
            return $"<b><i>{t}</i><b>";
#else
            return t;
#endif
        }
        
        
        public static string ToStringExtend(this Vector3 input)
        {
            return $"({input.x}, {input.y}, {input.z})";
        }

        /// <summary>
        /// reverse of Vector3 ToStringExtend
        /// </summary>
        public static Vector3 ToVector3(this string t)
        {
            // Remove the parentheses
            if (t.StartsWith ("(") && t.EndsWith (")")) {
                t = t.Substring(1, t.Length-2);
            }
 
            // split the items
            string[] a = t.Split(',');
 
            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(a[0]),
                float.Parse(a[1]),
                float.Parse(a[2]));
 
            return result;
        }
        
        public static Color ToColor(this string t)
        {
            ColorUtility.TryParseHtmlString(t, out var result);
            return result;
        }
        
        /// <summary>
        /// https://stackoverflow.com/questions/3453274/using-linq-to-get-the-last-n-elements-of-a-collection
        /// </summary>
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
        {
            return source.Skip(Mathf.Max(0, source.Count() - n));
        }

        public static List<T> TakeLast<T>(this List<T> source, int n)
        {
            return source.Skip(Mathf.Max(0, source.Count() - n)).ToList();
        }

        // public static void RemoveFrom<T>(this List<T> lst, int from)
        // {
        //     lst.RemoveRange(from, lst.Count - from);
        // }
        public static void RemoveLast<T>(this List<T> lst, int n)
        {
            int count = lst.Count;
            n = Mathf.Min(n, count);
            lst.RemoveRange(count - n, n);
        }
        
        public static string StripHTML(this string t)
        {
            return Regex.Replace(t, "<.*?>", string.Empty);
        }
        
#if UNITY_EDITOR
        public static void ToCoLog(this string s)
        {
            CoLogManager.Log(s);
        }
#endif

        public static string ToSringWithInt(this AnimationEvent e)
        {
            return $"{e.functionName} - {e.intParameter} - {e.time}";
        }
        #endregion string
        
        
        /// <summary>
        /// https://forum.unity.com/threads/contribution-texture2d-blur-in-c.185694/
        /// </summary>
        public static Texture2D MakeBlur(Texture2D image, int blurSize)
        {
            Texture2D blurred = new Texture2D(image.width, image.height);
     
            // look at every pixel in the blur rectangle
            for (int xx = 0; xx < image.width; xx++)
            {
                for (int yy = 0; yy < image.height; yy++)
                {
                    float avgR = 0, avgG = 0, avgB = 0, avgA = 0;
                    int blurPixelCount = 0;
     
                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + blurSize && x < image.width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.height); y++)
                        {
                            Color pixel = image.GetPixel(x, y);
     
                            avgR += pixel.r;
                            avgG += pixel.g;
                            avgB += pixel.b;
                            avgA += pixel.a;
     
                            blurPixelCount++;
                        }
                    }
     
                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;
                    avgA = avgA / blurPixelCount;
     
                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + blurSize && x < image.width; x++)
                        for (int y = yy; y < yy + blurSize && y < image.height; y++)
                            blurred.SetPixel(x, y, new Color(avgR, avgG, avgB, avgA));
                }
            }
            blurred.Apply();
            return blurred;
        }
        
        /// <summary>
        /// https://answers.unity.com/questions/752423/why-is-my-rendertexture-creating-a-pure-gray-textu.html
        /// </summary>
        public static Texture2D CaptureMainCamera(int mHeight, int antiAliasing)
        {
            Camera mCamera = Camera.allCameras[0];
            if (mCamera == null)
            {
                Debug.LogError("Extension CaptureMainCamera -mCamera NULL!");
                return null;
            }
            
            int mWidth = Mathf.RoundToInt(mCamera.aspect * mHeight);
            if (mWidth == 0)
            {
                Debug.LogError("Extension CaptureMainCamera -mWidth == 0!");
                return null;
            }
            Rect rect = new Rect(0, 0, mWidth, mHeight);
            RenderTexture renderTexture = new RenderTexture(mWidth, mHeight, 24)
            {
                antiAliasing = antiAliasing
            };
            if (renderTexture == null)
            {
                Debug.LogError("Extension CaptureMainCamera -renderTexture NULL!");
                return null;
            }
            //renderTexture.useDynamicScale = true;
        
            Texture2D screenShot = new Texture2D(mWidth, mHeight, TextureFormat.RGBA32, false);
            if (screenShot == null)
            {
                Debug.LogError("Extension CaptureMainCamera -screenShot NULL!");
                return null;
            }
            mCamera.targetTexture = renderTexture;
            //mCamera.forceIntoRenderTexture = true;
            mCamera.Render();
 
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(rect, 0, 0);
 
            mCamera.targetTexture = null;
            RenderTexture.active = null;
            //mCamera.forceIntoRenderTexture = false;
        
            screenShot.Apply(true);
            return screenShot;
        }
    }
}