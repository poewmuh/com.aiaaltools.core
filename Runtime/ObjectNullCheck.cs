using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace AiaalTools
{
    public static class ObjectNullCheck
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Ref<T>(this T o) where T : Object
        {
            return o.IsNull() ? null : o;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull(this Object o)
        {
            return !o;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull(this Object o)
        {
            return o;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InvokeIfNotNull<T>(this T src, Action<T> action) where T : Object
        {
            if (src.IsNull()) return false;
            action?.Invoke(src);
            return true;
        }
    }
}