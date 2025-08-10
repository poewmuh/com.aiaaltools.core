using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace AiaalTools.Data.Loader
{
    public class AssetLoaderHandler
    {
        private AsyncOperationHandle _handler;

        public T LoadGOImmediate<T>(string path) where T : Object
        {
            LoadInternal<GameObject>(path);
            _handler.WaitForCompletion();
            return (_handler.Result as GameObject).GetComponent<T>();
        }

        public T LoadImmediate<T>(string path) where T : Object
        {
            LoadInternal<T>(path);
            _handler.WaitForCompletion();
            return _handler.Result as T;
        }

        private void LoadInternal<T>(string path)
        {
            _handler = Addressables.LoadAssetAsync<T>(path);
        }

        public void Unload()
        {
            Addressables.Release(_handler);
        }
    }
}