using UnityEngine;

namespace AiaalTools.Data.Saver
{
    public class LocalDataSaver
    {
        private const string JsonExtension = ".json";
        private readonly string _path;

        public LocalDataSaver(string name, object target)
        {
            _path = $"{ProfileData.pathLocalDatas}{name}{JsonExtension}";
        }

        public void Save(string json)
        {
            System.IO.File.WriteAllText(_path, json);
            Debug.Log("Settings saved to: " + _path);
        }
    }
}