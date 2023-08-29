using Newtonsoft.Json;
using ParserNansemAPI.Class.Patterns;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ParserNansemAPI.Class.settings
{
    class ConfigurationManager
    {
        private static string path = "";
        private static List<ExcelTable> list = new List<ExcelTable>();
        static ConfigurationManager()=> path = Directory.GetCurrentDirectory();
        public Settings GetCurrentSettings() => File.Exists($"{path}/Settings/settings.json") ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText($"{path}/Settings/settings.json")) : null;
        public void SaveCurrentSettings(in Settings settings) => File.WriteAllText($"{path}/Settings/settings.json", JsonConvert.SerializeObject(settings));
        public bool IsIPAddress(string ip) => IPAddress.TryParse(ip, out IPAddress ipAddress);
        public bool InternetAvalible()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.google.com");
            try
            {
               HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }
        public void SaveData(ExcelTable newData) => list.Add(newData);

        public int GetCount() => list.Count;
        public List<ExcelTable> GetData() => list;
        
    }
}
