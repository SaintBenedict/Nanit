using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using NaNiT.Functions;

namespace NaNiT
{
    class Config
    {
        internal static string ConfigPath { get { return Path.Combine(MainProgram.SavePath, "config.json"); } }

        public static void CreateIfNot(string file, string data = "")
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, data);
            }
        }

        public static string ReadConfigFile(string file)
        {
            return File.ReadAllText(file);
        }

        public static void SetupConfig()
        {
            if (!Directory.Exists(MainProgram.SavePath))
            {
                Directory.CreateDirectory(MainProgram.SavePath);
            }

            if (File.Exists(ConfigPath))
            {
                MainProgram.config = ConfigFile.Read(ConfigPath);
            }

            if (MainProgram.config == null)
                MainProgram.config = new ConfigFile();

            MainProgram.config.Write(ConfigPath);
        }
    }

    class ConfigFile
    {
        [Description("")]
        public short serverPort = 32718;
        public string logFile = "server.log";

        public LogType logLevel = LogType.Info;
        
        
        

        public bool useAssetDigest = false;
        public string assetDigest = "8168975B43CBB5A002D3CCBD41FAFD226D3F58ECC3A6F835C26980531EF6AA6C";

        public static ConfigFile Read(string path)
        {
            if (!File.Exists(path))
                return new ConfigFile();

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ConfigFile file = Read(fs);
                MainProgram.logInfo("Файл конфигурации загружен");
                return file;
            }
        }

        public static ConfigFile Read(Stream stream)
        {
            try
            {
                using (var sr = new StreamReader(stream))
                {
                    return JsonConvert.DeserializeObject<ConfigFile>(sr.ReadToEnd());
                }
            }
            catch (Exception)
            {
                MainProgram.logException("Файл конфигурации не прочитать. Будет создан новый.");
                return new ConfigFile();
            }
        }

        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fs);
            }
        }

        public void Write(Stream stream)
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }
    }
}
