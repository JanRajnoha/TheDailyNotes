using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Framework.Classes
{
    public class Constants
    {
        [XmlIgnore]
        [JsonIgnore]
        public bool BetaMode { get; set; } = false;
        [XmlIgnore]
        [JsonIgnore]
        public bool ConstantsSetComplete { get; set; } = false;
        [XmlElement(IsNullable = true)]
        public bool? LogEnable { get; set; }
        [XmlElement(IsNullable = true)]
        public bool? Debug { get; set; }
        [XmlElement(IsNullable = true)]
        public bool? AppFail { get; set; }

        bool saveSettings = false;

        public static Constants Instance = new Constants();

        public static async void SetConstants(bool betaMode = false, bool logEnable = false, bool debug = false, bool appFail = false)
        {
            if (Instance.ConstantsSetComplete)
                return;

            Instance.ConstantsSetComplete = true;

            Instance = await LoadSettingsAsync();

            Instance.BetaMode = betaMode;

            if (Instance.LogEnable == null)
                Instance.LogEnable = logEnable;

            if (Instance.Debug == null)
                Instance.Debug = debug;

            if (Instance.AppFail == null)
                Instance.AppFail = appFail;

            if (Instance.saveSettings)
                await SaveSettingsAsync();
        }

        public static async Task<Constants> LoadSettingsAsync()
        {
            var loadResult = await Storage.FileStream<Constants>.ReadDataAsync("Set.dat", "");

            if (loadResult == null)
            {
                return new Constants()
                {
                    saveSettings = true
                };
            }

            return loadResult;
        }

        public static Constants LoadSettings()
        {
           return LoadSettingsAsync().Result;
        }

        public static async Task SaveSettingsAsync()
        {
            Constants con = new Constants();

            con = Instance;

            await Storage.FileStream<Constants>.SaveDataAsync(con, "Set.dat", "");
        }

        public static void SaveSettings()
        {
            SaveSettingsAsync().Wait();
        }
    }
}
