using UnityEngine;

namespace AiaalTools
{
    public static class ExtensionMethods
    {
        public static Vector3 OnlyXZ(this Vector3 v3)
        {
            v3.y = 0f;
            return v3;
        }
        
        public static float Sqr(this float f)
        {
            return f * f;
        }

        public static bool ApproxZero(this float f)
        {
            return Mathf.Approximately(0, f);
        }
        
        public static float Abs(this float f)
        {
            return Mathf.Abs(f);
        }

        public static bool Approx(this float f, float f2)
        {
            return Mathf.Approximately(f, f2);
        }
    }
}