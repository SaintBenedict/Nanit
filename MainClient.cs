using System;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;
using static NaNiT.GlobalVariable;
using static NaNiT.GlobalFunctions;
using static NaNiT.LocalGlobals;
using NaNiT.Utils;
using static NaNiT.Utils.Functions;
using System.Diagnostics;
using System.Net;
using NaNiT.Permissions;
using System.IO;

namespace NaNiT
{
    class MainClient
    {
        public static NotifyIcon TrayNotify = null;
        public static TimerCallback Upd = new TimerCallback(CheckServiceUpdate);

        public static AttemptingServer ServFinder;
        static Thread MainClientThread;
        static Thread ProblemMonitorThread;
        
        public static _ClientState StateMyActtivity;
        public static ServChecker ServerStatus;
        public static LogType logLevel = LogType.Info;

        private static void ProcessExit(object sender, EventArgs e)
        {
            Process This = Process.GetCurrentProcess();
            This.Kill();
        }

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            TrayNotify = new NotifyIcon()
            {
                Icon = Resources.net1,
                Visible = true,
                ContextMenuStrip = new ContextMenus().Create(),
                Text = "Сетевой клиент НИИ Телевидения"
            };
            
            StateMyActtivity = _ClientState.OfflineWork;
            ServerStatus = ServChecker.NotConnecting;

            Thread MyName = Thread.CurrentThread;
            if (MyName.Name == null)
                MyName.Name = "Основной поток";

            ProblemMonitorThread = new Thread(new ThreadStart(ProblemMonitor));
            ProblemMonitorThread.Start();

            gl_s_OSdate = GetOSDate();
            // Проверка наличия настроек в реестре // Обязательно после ГетОСДаты, она там используется
            RegCheck();

            gl_s_myHostName = Dns.GetHostName();
            gl_f_soft = new FormSoft();
            gl_s_userName = gl_s_myHostName + @"*" + gl_s_OSdate.Substring(4, 2) + gl_s_OSdate.Substring(2, 2);

            // Копирование себя в папку сервисов, запуск оттуда удаление лишнего
            AutoSelfInstall(false);

            writeLog("", LogType.FileOnly);
            writeLog("-- Log Start: " + DateTime.Now + " --", LogType.FileOnly);

            ServFinder = new AttemptingServer();
            MainClientThread = new Thread(new ThreadStart(ServFinder.Run));
            if (MainClientThread.Name == null)
                MainClientThread.Name = "Попытка подлючиться";
            MainClientThread.Start();
            
            // Старт таймеров и тредов
            Timer UpdateTimer = new Timer(Upd, null, 0, 3000000);
            
            //
            Application.Run();
        }

        public static void ProblemMonitor()
        {
            while (true)
            {
                Thread MyThread = Thread.CurrentThread;
                if (MyThread.Name == null)
                    MyThread.Name = "Поток монитора состояний";
                if (StateMyActtivity == _ClientState.Crashing || ServerStatus == ServChecker.Crashing)
                {
                    logFatal("Критическая ошибка на одной из сторон вызвала отключение.");
                    if (ServFinder.newConnectThread != null)
                    {
                        ServFinder.newConnectThread.Abort();
                        ServFinder.newConnectThread = null;
                    }
                    if (ServFinder.StreamApp != null)
                    {
                        ServFinder.StreamApp.Close();
                        ServFinder.StreamApp = null;
                    }
                    if (ServFinder.NewConnection != null)
                    {
                        ServFinder.NewConnection.Close();
                        ServFinder.NewConnection = null;
                    }
                    
                    AttemptingServer.connections.Clear();
                    ServerStatus = ServChecker.NotConnecting;
                    StateMyActtivity = _ClientState.OfflineWork;
                    TrayNotify.Icon = Resources.net1;
                    Thread.Sleep(10000);
                    break;
                }
                Thread.Sleep(2000);
            }
        }
        

        public static void logDebug(string source, string message) { writeLog("[" + source + "]:" + message, LogType.Debug); }
        public static void logInfo(string message) { writeLog(message, LogType.Info); }
        public static void logError(string message) { writeLog(message, LogType.Error); }
        public static void logException(string message) { writeLog(message, LogType.Exception); }
        public static void logFatal(string message) { writeLog(message, LogType.Fatal); }
        public static void writeLog(string message, LogType logType)
        {
            if ((int)logLevel > (int)logType && logType != LogType.FileOnly) return;

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
                using (StreamWriter w = File.AppendText(Path.Combine("", "client_log.txt")))
                {
                    w.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                if (logLevel == LogType.Debug) Console.WriteLine("[DEBUG] Logger Exception: " + e.ToString());
            }

            if ((int)logType >= (int)logLevel) Console.WriteLine(message);
        }

        public static void CheckServiceUpdate(object obj)
        {
            Thread MyThread = Thread.CurrentThread;
            if (MyThread.Name == null)
                MyThread.Name = "Update Timer";

            ServiceWork.CheckUpdServer();
            ServiceWork.ServiceInit();
            Thread.Sleep(5000);

            if (gl_i_adrUpdNum != -1)
                if (gl_s_nanitSvcVer != gl_s_updVerAvi)
                {
                    ServiceWork.UpdateService();
                }
        }
    }
}