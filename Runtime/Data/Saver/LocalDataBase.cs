using Newtonsoft.Json;

namespace AiaalTools.Data.Saver
{
    public class LocalDataBase
    {
        [JsonIgnore] private string _name = string.Empty;
        [JsonIgnore] private LocalDataSaver _dataSaver;

        [JsonIgnore]
        public string name
        {
            get => _name;
            set => _name = value;
        }

        public virtual void OnGenerate() { }
        public virtual void OnLoaded() 
        {
            _dataSaver = new LocalDataSaver(_name, this);
        }

        public virtual void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            _dataSaver.Save(json);
        }

        public virtual void ResetAndSave()
        {
            OnGenerate();
            Save();
        }
    }
}