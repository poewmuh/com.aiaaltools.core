using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiaalTools
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}
