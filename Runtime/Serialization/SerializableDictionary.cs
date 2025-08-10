using System.Collections.Generic;
using UnityEngine;

namespace AiaalTools.Serialization
{
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private TKey[] _keys;
        [SerializeField] private TValue[] _values;

        public TValue GetValueOrDefault(TKey key)
        {
            return ContainsKey(key) ? this[key] : default;
        }

        public void OnAfterDeserialize()
        {
            if (_keys != null && _values != null && _keys.Length == _values.Length)
            {
                Clear();
                var n = _keys.Length;
                for (int i = 0; i < n; ++i)
                {
                    var key = _keys[i];
                    if (key == null)
                    {
                        Debug.LogError($"{GetType()} - null key found in serialized dict!");
                    }

                    this[key] = _values[i];
                }

                _keys = null;
                _values = null;
            }

        }

        public void OnBeforeSerialize()
        {
            _keys = new TKey[Count];
            _values = new TValue[Count];

            int i = 0;
            foreach (var kvp in this)
            {
                _keys[i] = kvp.Key;
                _values[i] = kvp.Value;
                ++i;
            }
        }
    }
}