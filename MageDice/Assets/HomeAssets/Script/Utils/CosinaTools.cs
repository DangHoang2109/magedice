using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosina.Components
{
    public static class CosinaTools
    {
        public static readonly Color[] COLORS_LIGHT_GOOD = new[]
        {
            Color.red,
            Color.green,
            Color.magenta,
            Color.cyan,
            Color.yellow,
            Color.grey,
            Color.white
        };
        
        public static readonly Color[] COLORS_DARK_GOOD = new[]
        {
            Color.black,
            Color.red,
            Color.blue,
            Color.grey,
            Color.magenta,
            new Color(0f, 0.5f, 0f),
            new Color(0.4f, 0.4f, 0.05f),
            new Color(0.4f, 0.1f, 0.05f),
        };
        
        public static Texture2D CreateTextureWithColor(Color col)
        {
            Texture2D texture = new Texture2D(1, 1);
            Color[] cols = new Color[1];
            cols[0] = col;
            texture.SetPixels(cols);
            return texture;
        }
        public static Texture2D CreateTextureWithColor(int width, int height, Color col)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] cols = new Color[width * height];
            for (int i = 0; i < cols.Length; ++i)
            {
                cols[i] = col;    
            }
            texture.SetPixels(cols);
            return texture;
        }

        public static Color RandomColor(bool isOverDark)
        {
            if (isOverDark)
                return COLORS_LIGHT_GOOD.Random();
            return COLORS_DARK_GOOD.Random();
        }
        
        
#if UNITY_EDITOR

        public static void GizmosDrawDiamond(Vector3 pos, Color color, float diagonalSize)
        {
            diagonalSize /= 2f;
            Gizmos.color = color;

            Gizmos.DrawLine(pos + Vector3.up * diagonalSize * 0.98f, pos + Vector3.right * diagonalSize * 0.98f);
            Gizmos.DrawLine(pos + Vector3.up * diagonalSize * 1.02f, pos + Vector3.right * diagonalSize * 1.02f);

            Gizmos.DrawLine(pos - Vector3.up * diagonalSize * 0.98f, pos - Vector3.right * diagonalSize * 0.98f);
            Gizmos.DrawLine(pos - Vector3.up * diagonalSize * 1.02f, pos - Vector3.right * diagonalSize * 1.02f);

            Gizmos.DrawLine(pos + Vector3.up * diagonalSize * 0.98f, pos - Vector3.right * diagonalSize * 0.98f);
            Gizmos.DrawLine(pos + Vector3.up * diagonalSize * 1.02f, pos - Vector3.right * diagonalSize * 1.02f);

            Gizmos.DrawLine(pos - Vector3.up * diagonalSize * 0.98f, pos + Vector3.right * diagonalSize * 0.98f);
            Gizmos.DrawLine(pos - Vector3.up * diagonalSize * 1.02f, pos + Vector3.right * diagonalSize * 1.02f);
        }

        public static void GizmosDrawCircle(Vector3 pos, Color color, float radius)
        {
            Gizmos.color = color;

            Gizmos.DrawWireSphere(pos, radius * 0.98f);
            Gizmos.DrawWireSphere(pos, radius * 1.02f);
        }

        public static void GizmosDrawBox(Vector3 pos, Color color, float verticalSize)
        {
            Gizmos.color = color;

            Gizmos.DrawWireCube(pos, Vector3.one * verticalSize * 0.98f);
            Gizmos.DrawWireCube(pos, Vector3.one * verticalSize * 1.02f);
        }

#endif
    }

}
