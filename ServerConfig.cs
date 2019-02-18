using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;

namespace NaNiT
{
    class ServerConfig
    {
        internal static string ConfigPath { get { return Application.StartupPath; } }

        public static void CreateIfNot(string file, string data = "")
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, data);
            }
        }

        public static void SetupConfig()
        {
            if (File.Exists(ConfigPath))
            {
                MainProgram.serverConfig = ServerFile.Read(ConfigPath);
            }
            MainProgram.serverConfig.Write(ConfigPath);
        }
    }

    [Serializable]
    class ServerFile
    {
        public int serverPort = 51780;
       
        public bool allowAdminCommandsFromAnyone = false;
        
        public bool attemptAuthentication = false;



        public static ServerFile Read(string path)
        {
            if (!File.Exists(path))
                return new ServerFile();
            
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ServerFile file = Read(fs);
                MainProgram.logInfo("Конфиг сервера успешно загружен");
                return file;
            }
        }

        public static ServerFile Read(Stream stream)
        {
            try
            {
                SoapFormatter formatter = new SoapFormatter();
                ServerFile str = (ServerFile)formatter.Deserialize(stream);
                return str as ServerFile;
            }
            catch (Exception)
            {
                MainProgram.logException("Конфиг нечитаем, делаем новый");
                return new ServerFile();
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
            SoapFormatter formatter = new SoapFormatter();
            formatter.Serialize(stream, this);
        }
    }
}
