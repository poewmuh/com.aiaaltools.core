using UnityEngine;

namespace AiaalTools
{
    public static class ColorExt
    {
        public static void Lerp(ref Color color, Color a, Color b, float t)
        {
            t = Mathf.Clamp01(t);
            LerpUnclamped(ref color, a, b, t);
        }
        
        public static void LerpUnclamped(ref Color color, Color a, Color b, float t)
        {
            color.r = a.r + (b.r - a.r) * t;
            color.g = a.g + (b.g - a.g) * t;
            color.b = a.b + (b.b - a.b) * t;
            color.a = a.a + (b.a - a.a) * t;
        }

        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}