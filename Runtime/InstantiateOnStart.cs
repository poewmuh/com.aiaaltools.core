using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiaalTools
{
    public class InstantiateOnStart : MonoBehaviour
    {
        [SerializeField] private GameObject[] _prefabs;

        private void Start()
        {
            foreach (var prefab in _prefabs)
            {
                if (!prefab)
                {
                    continue;
                }

                Instantiate(prefab);
            }
        }
    }
}

