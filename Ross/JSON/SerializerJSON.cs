using System.IO;
using Newtonsoft.Json;

namespace Ross.JSON
{
    public static class SerializerJSON
    {
        internal static void Serialize<T>(this T arg, string fileName)
        {
            var res = JsonConvert.SerializeObject(arg, Formatting.Indented);
            File.WriteAllText(fileName, res);
        }

        internal static T Deserialize<T>(string fileName)
        {
            var json = File.Exists(fileName) ? File.ReadAllText(fileName) : string.Empty;
            var res = JsonConvert.DeserializeObject<T>(json);

            return res;
        }
    }
}