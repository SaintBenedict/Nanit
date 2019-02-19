using NaNiT.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;

namespace NaNiT
{
    class ServerConfig
    {
        internal static string ConfigPath { get { return @"Server.config"; } }

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
                MainProgram.ServerConfig = ServerFile.Read(ConfigPath);
            }
            MainProgram.ServerConfig.Write(ConfigPath);
        }
    }

    [Serializable]
    class ServerFile
    {
        public int serverPort = 51780;
        public bool attemptAuthentication = false;
        public bool TrayMenuIsOpen = false;
        public bool ServerFormIsOpen = false;
        public string logFile = "server.log";
        public LogType logLevel = LogType.Info;

        public static ServerFile Read(string path)
        {
            if (!File.Exists(path))
                return new ServerFile();
            
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ServerFile file = Read(fs);
                MainProgram.logInfo("Файл настроек прочитан");
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
                MainProgram.logException("Не удалось прочитать файл настроек. Создаём новый.");
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
