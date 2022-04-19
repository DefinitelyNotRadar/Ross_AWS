using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ross.JSON
{
    public static class SerializerJSON
    {
        internal static void Serialize<T>(this T arg, string fileName)
        {
            string res = JsonConvert.SerializeObject(arg, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(fileName, res);
        }

        internal static T Deserialize<T>(string fileName)
        {

            string json = (File.Exists(fileName)) ? File.ReadAllText(fileName) : string.Empty;
            T res = JsonConvert.DeserializeObject<T>(json);

            return res;

        }
    }
}
