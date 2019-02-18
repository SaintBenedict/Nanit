using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NaNiT.Permissions;
using NaNiT.Functions;

namespace NaNiT
{
    class MainProgram
    {
        public static readonly string SavePath = Application.StartupPath;
        public static ConfigFile config = new ConfigFile();
        public static ServerFile serverConfig = new ServerFile();

        public static Dictionary<string, ClientThread> clients = new Dictionary<string, ClientThread>();
        public static int clientCount { get { return clients.Count; } set { return; } }

        public static ServerThread MainServer;
        static Thread MainServerThread;
        static Thread monitorThread;

        public static bool allowNewClients = true;

        public static ServerState serverState;

        public static int startTime;
        public static int restartTime = 0;

        public static string defaultGroup = null;
        

        private static void ProcessExit(object sender, EventArgs e)
        {
            Process This = Process.GetCurrentProcess();
            This.Kill();
        }

        static void Main(string[] args)
        {
            startTime = Function.getTimestamp();
            serverState = ServerState.Starting;
            
            monitorThread = new Thread(new ThreadStart(MainProgram.crashMonitor));
            monitorThread.Name = "Монитор состояния";
            monitorThread.Start();

            Config.SetupConfig();
            ServerConfig.SetupConfig();
            Users.SetupUsers();

            serverConfig.Write(ServerConfig.ConfigPath);
            writeLog("", LogType.FileOnly);
            writeLog("-- Log Start: " + DateTime.Now + " --", LogType.FileOnly);
            
            MainServer = new ServerThread();
            MainServerThread = new Thread(new ThreadStart(MainServer.run));
            MainServerThread.Start();

            logInfo("Запуск сервера...");
            while (serverState != ServerState.Running) { if (serverState == ServerState.Crashed) return; }
            logInfo("Все приготовления выполнены. Сервер работает.");
        }

        public static void crashMonitor()
        {
            while (true)
            {
                if (restartTime != 0)
                {
                    if (restartTime < Function.getTimestamp()) doRestart();
                }

                if (serverState == ServerState.Crashed)
                {
                    logFatal("Фатальная ошибка на сервере. Перезапуск через 10 секунд.");
                    Thread.Sleep(10000);
                    doRestart();
                    break;
                }
                Thread.Sleep(2000);
            }
        }

        public static void doRestart()
        {
            serverState = ServerState.Restarting;

            foreach (ClientThread client in clients.Values)
            {
                client.sendServerPacket(Packet.ClientDisconnect, new byte[1]);
                client.sendChatMessage("^#f75d5d;You have been disconnected.");
                client.clientState = ClientState.Disposing;
                client.kickTargetTimestamp = Function.getTimestamp() + 7;
            }

            while (clients.Count > 0)
            {
                // Waiting
            }

            if (MainServerThread != null) MainServerThread.Abort();
            Thread.Sleep(500);
            logInfo("Поток завершился, перезапускаем.");
            Thread.Sleep(3000);

            Process.Start(Environment.CurrentDirectory + "\\NanServer.exe");
            Environment.Exit(1);
        }

        public static void logDebug(string source, string message) { writeLog("[" + source + "]:" + message, LogType.Debug); }

        public static void logInfo(string message) { writeLog(message, LogType.Info); }

        public static void logWarn(string message) { writeLog(message, LogType.Warn); }

        public static void logError(string message) { writeLog(message, LogType.Error); }

        public static void logException(string message) { writeLog(message, LogType.Exception); }

        public static void logFatal(string message) { writeLog(message, LogType.Fatal); }

        public static void writeLog(string message, LogType logType)
        {
            if ((int)config.logLevel > (int)logType && logType != LogType.FileOnly) return;

            switch (logType)
            {
                case LogType.Debug:
                    message = "[DEBUG] " + message;
                    break;

                case LogType.Info:
                    message = "[INFO] " + message;
                    break;

                case LogType.Warn:
                    message = "[WARN] " + message;
                    break;

                case LogType.Error:
                    message = "[ERROR] " + message;
                    break;

                case LogType.Exception:
                    message = "[EXCEPTION] " + message;
                    break;

                case LogType.Fatal:
                    message = "[FATAL ERROR] " + message;
                    break;
            }

            try
            {
                using (StreamWriter w = File.AppendText(Path.Combine(MainProgram.SavePath, "log.txt")))
                {
                    w.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                if (config.logLevel == LogType.Debug) Console.WriteLine("[DEBUG] Logger Exception: " + e.ToString());
            }

            if ((int)logType >= (int)config.logLevel) Console.WriteLine(message);
        }
        

        public static void sendGlobalMessage(string message)
        {
            foreach (ClientThread client in clients.Values)
            {
                client.sendChatMessage("^#5dc4f4;" + message);
            }
        }

        public static void sendGlobalMessage(string message, string color)
        {
            foreach (ClientThread client in clients.Values)
            {
                client.sendChatMessage("^" + color + ";" + message);
            }
        }
    }
}
