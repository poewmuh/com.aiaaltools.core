using System.Collections.Generic;
using UnityEngine;

namespace AiaalTools
{
    public static class YieldUtils
    {
       private static readonly Dictionary<int, WaitForSeconds> _cache = new Dictionary<int, WaitForSeconds>();

	   public static WaitForSeconds WaitForSeconds(float seconds)
	   {
		   return WaitForMilliseconds((int) (seconds * 1000.0f));
	   }
	   
	   public static WaitForSeconds WaitForMilliseconds(int milliseconds)
	   {
		   if (!_cache.TryGetValue(milliseconds, out var value))
		   {
			   value = new WaitForSeconds(milliseconds / 1000.0f);
			   _cache.Add(milliseconds, value);
		   }

		   return value;
	   }
    }
}