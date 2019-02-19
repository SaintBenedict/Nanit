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
        public static ServerFile ServerConfig = new ServerFile();
        public static NotifyIcon TrayNotify;
        public static FormSOptions ServerForm;

        public static Dictionary<string, ClientThread> ActiveClients = new Dictionary<string, ClientThread>();
        public static int CountOfActiveClients { get { return ActiveClients.Count; } set { return; } }

        public static ServerThread MainServer;
        static Thread MainServerThread;
        static Thread CrashMonitorThread;

        public static bool AllowNewClients = true;

        public static ServerState CurrentServerStatus;

        public static int StartTime;
        public static int RestartTime = 0;

        private static void ProcessExit(object sender, EventArgs e)
        {
            TrayNotify.Icon.Dispose();
            TrayNotify.Dispose();
            Process This = Process.GetCurrentProcess();
            This.Kill();
        }

        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TrayNotify = new NotifyIcon()
            {
                Icon = Resources.net4,
                Visible = true,
                ContextMenuStrip = new ContextMenus().Create(),
                Text = "Сетевой сервер НИИ Телевидения"
            };
            ServerForm = new FormSOptions();
            StartTime = Function.getTimestamp();
            CurrentServerStatus = ServerState.Starting;
            
            CrashMonitorThread = new Thread(new ThreadStart(СrashMonitor));
            if(CrashMonitorThread.Name == null)
                CrashMonitorThread.Name = "Поток монитора состояний";
            CrashMonitorThread.Start();

            NaNiT.ServerConfig.SetupConfig();
            Users.SetupUsers();

            ServerConfig.Write(NaNiT.ServerConfig.ConfigPath);
            writeLog("", LogType.FileOnly);
            writeLog("-- Log Start: " + DateTime.Now + " --", LogType.FileOnly);
            
            MainServer = new ServerThread();
            MainServerThread = new Thread(new ThreadStart(MainServer.Run));
            if (MainServerThread.Name == null)
                MainServerThread.Name = "Поток запуска сервера";
            MainServerThread.Start();
            
            while (CurrentServerStatus != ServerState.Running) { if (CurrentServerStatus == ServerState.Crashed) return; }
            TrayNotify.Icon = Resources.net3;
            logInfo("Все приготовления выполнены. Сервер работает.");

            Application.Run();
        }

        public static void СrashMonitor()
        {
            while (true)
            {
                if (RestartTime != 0)
                {
                    if (RestartTime < Function.getTimestamp()) RestartingServer();
                }

                if (CurrentServerStatus == ServerState.Crashed)
                {
                    logFatal("Фатальная ошибка на сервере. Перезапуск через 10 секунд.");
                    Thread.Sleep(10000);
                    RestartingServer();
                    break;
                }
                Thread.Sleep(2000);
            }
        }
        public static void StopServer()
        {
            foreach (ClientThread client in ActiveClients.Values)
            {
                client.SendServerPacket(Packet.ClientDisconnect, new byte[1]);
                client.statusOfCurrentClient = ClientState.Disposing;
                client.kickTargetTimestamp = Function.getTimestamp() + 7;
            }
            TrayNotify.Icon = Resources.net4;
            while (ActiveClients.Count > 0)
            {
                // Waiting
            }

            if (MainServerThread != null) MainServerThread.Abort();
            Thread.Sleep(500);
        }

        public static void StartServer()
        {
            CurrentServerStatus = ServerState.Starting;
            RestartTime = 0;
            MainServer = null;
            StartTime = Function.getTimestamp();
            Users.SetupUsers();
            writeLog("", LogType.FileOnly);
            writeLog("-- Log Start [After Restart]: " + DateTime.Now + " --", LogType.FileOnly);
            MainServer = new ServerThread();
            MainServerThread.Start();
            CrashMonitorThread.Start();
            while (CurrentServerStatus != ServerState.Running) { if (CurrentServerStatus == ServerState.Crashed) return; }
            TrayNotify.Icon = Resources.net3;
            logInfo("Все приготовления выполнены. Сервер работает.");
        }

        public static void RestartingServer()
        {
            CurrentServerStatus = ServerState.Restarting;
            StopServer();
            logInfo("Поток завершился, перезапускаем.");
            Thread.Sleep(3000);
            StartServer();
        }

        public static void logDebug(string source, string message) { writeLog("[" + source + "]:" + message, LogType.Debug); }
        public static void logInfo(string message) { writeLog(message, LogType.Info); }
        public static void logError(string message) { writeLog(message, LogType.Error); }
        public static void logException(string message) { writeLog(message, LogType.Exception); }
        public static void logFatal(string message) { writeLog(message, LogType.Fatal); }
        public static void writeLog(string message, LogType logType)
        {
            if ((int)ServerConfig.logLevel > (int)logType && logType != LogType.FileOnly) return;

            switch (logType)
            {
                case LogType.Debug:
                    message = "[DEBUG] " + message;
                    break;

                case LogType.Info:
                    message = "[INFO] " + message;
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
                using (StreamWriter w = File.AppendText(Path.Combine(SavePath, "log.txt")))
                {
                    w.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                if (ServerConfig.logLevel == LogType.Debug) Console.WriteLine("[DEBUG] Logger Exception: " + e.ToString());
            }

            if ((int)logType >= (int)ServerConfig.logLevel) Console.WriteLine(message);
        }
    }
}
