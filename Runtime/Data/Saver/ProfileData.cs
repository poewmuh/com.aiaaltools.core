using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AiaalTools.Data.Saver
{
    public static class ProfileData
    {
        private static string fileExtension = ".json";

        private static Dictionary<System.Type, LocalDataBase> _localProfileData = new();

        private static bool _isInited;

        public static string pathLocalDatas { get; private set; }

        public static void InitializeFromClient()
        {
            InitLocalPath();
            InitLocalDatas();
        }

        private static void InitLocalDatas()
        {
            InitLocalData<ApplicationSettingsData>(ApplicationSettingsData.NAME);
        }

        private static void InitLocalPath()
        {
            var mainPath = Application.persistentDataPath + @"/GameFolder/";
            if (!Directory.Exists(mainPath))
            {
                Directory.CreateDirectory(mainPath);
            }

            pathLocalDatas = mainPath + "LocalDatas/";
            if (!Directory.Exists(pathLocalDatas))
            {
                Directory.CreateDirectory(pathLocalDatas);
            }
        }

        private static void InitLocalData<T>(string name) where T : LocalDataBase, new()
        {
            T profileData;
            string pathFile = pathLocalDatas + name + fileExtension;
            if (File.Exists(pathFile))
            {
                var text = File.ReadAllText(pathFile);
                try
                {
                    profileData = JsonConvert.DeserializeObject<T>(text);
                    profileData.name = name;
                    profileData.OnLoaded();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[InitLocalData] {name} {e}");
                    profileData = GenerateLocalData<T>(name);
                }

            }
            else
            {
                Debug.Log($"[InitLocalData] {name} doesn't exist");
                profileData = GenerateLocalData<T>(name);
            }

            _localProfileData[profileData.GetType()] = profileData;
        }

        private static T GenerateLocalData<T>(string name) where T : LocalDataBase, new()
        {
            T profileData = new T();
            profileData.name = name;
            profileData.OnGenerate();
            profileData.OnLoaded();
            profileData.Save();
            return profileData;
        }

        public static T GetLocalData<T>() where T : LocalDataBase
        {
            if (_localProfileData.ContainsKey(typeof(T)))
                return (T)_localProfileData[typeof(T)];
            return null;
        }

        public static bool TryGetLocalData<T>(out T result) where T : LocalDataBase
        {
            if (_localProfileData.TryGetValue(typeof(T), out var res))
            {
                result = (T)res;
                return true;
            }

            result = null;
            return false;
        }

        public static void ResetAllLocalDatas()
        {
            foreach (var item in _localProfileData)
            {
                string pathFile = pathLocalDatas + item.Value.name + fileExtension;
                if (File.Exists(pathFile))
                    item.Value.ResetAndSave();
            }
        }

        public static void DeleteAllLocalDatas()
        {
            foreach (var item in _localProfileData)
            {
                string pathFile = pathLocalDatas + item.Value.name + fileExtension;
                if (File.Exists(pathFile))
                {
                    try
                    {
                        File.Delete(pathFile);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[DeleteAllLocalDatas] Error deleting {pathFile}: {e}");
                    }
                }
            }

            _localProfileData.Clear();
        }

        public static void ResetStatic()
        {
            _isInited = default;
            _localProfileData.Clear();
        }
    }
}