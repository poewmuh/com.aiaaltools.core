using UnityEngine;

namespace AiaalTools
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance => _instance as T;
        private static MonoSingleton<T> _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Debug.LogError($"{typeof(T).Name} singleton duplication in {gameObject.name}");
                Destroy(this);
            }

            Init();
        }
        
        protected virtual void Init() { }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}