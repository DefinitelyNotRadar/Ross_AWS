using ModelsTablesDBLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YamlDotNet.Serialization;

namespace Ross
{
    public partial class MainWindow : Window
    {
        public LocalProperties YamlLoad()
        {
            string text = "";
            try
            {
                using (StreamReader sr = new StreamReader("LocalProperties.yaml", System.Text.Encoding.Default))
                {
                    text = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            var deserializer = new DeserializerBuilder().Build();

            var localProperties = new LocalProperties();
            try
            {
                localProperties = deserializer.Deserialize<LocalProperties>(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (localProperties == null)
            {
                localProperties = GenerateDefaultLocalProperties();
                YamlSave(localProperties);
            }
            return localProperties;
        }

        private LocalProperties GenerateDefaultLocalProperties()
        {
            var localProperties = new LocalProperties();
            localProperties.ADSB.IpAddress = "192.168.0.11";
            localProperties.ADSB.Port = 30005;
            localProperties.DbServer.IpAddress = "192.168.0.102";
            localProperties.DbServer.Port = 8302;
            localProperties.DspServer.IpAddress = "192.168.0.102";
            localProperties.DspServer.Port = 10001;
            localProperties.EdServer.IpAddress = "192.168.0.102";
            localProperties.EdServer.Port = 10009;

            localProperties.PCProperties.PortPPC = 9101;
            localProperties.PCProperties.PortRouter = 9101;
            localProperties.Spoofing.Port = 9114;

            return localProperties;
        }

        public T YamlLoad<T>(string NameDotYaml) where T : new()
        {
            string text = "";
            try
            {
                using (StreamReader sr = new StreamReader(NameDotYaml, System.Text.Encoding.Default))
                {
                    text = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            var deserializer = new DeserializerBuilder().Build();

            var t = new T();
            try
            {
                t = deserializer.Deserialize<T>(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //if (t == null)
            //{
            //    t = new T();
            //    YamlSave(t, NameDotYaml);
            //}
            return t;
        }

        public void YamlSave(LocalProperties localProperties)
        {
            try
            {
                var builder = new SerializerBuilder();
                builder.EmitDefaults();
                var serializer = builder.Build();
                var yaml = serializer.Serialize(localProperties);

                using (StreamWriter sw = new StreamWriter("LocalProperties.yaml", false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(yaml);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void YamlSave<T>(T t, string NameDotYaml) where T : new()
        {
            try
            {
                var builder = new SerializerBuilder();
                builder.EmitDefaults();
                var serializer = builder.Build();
                var yaml = serializer.Serialize(t);

                using (StreamWriter sw = new StreamWriter(NameDotYaml, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(yaml);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
