using Newtonsoft.Json;

namespace AiaalTools.Data.Saver
{
    public class ApplicationSettingsData : LocalDataBase
    {
        public const string NAME = "local_data_app_settings";

        [JsonProperty] private float masterVolume = 1f;

        [JsonIgnore]
        public float MasterVolume
        {
            get => masterVolume;
            set => masterVolume = value;
        }

        public override void OnGenerate()
        {
            base.OnGenerate();
            masterVolume = 0.7f;
        }
    }
}