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
    class LocalGlobals
    {
        //-Forms-//
        public static FormLogin gl_f_login = null;
        public static FormOptions gl_f_options = null;
        public static FormUpdater gl_f_updater = null;
        public static FormSoft gl_f_soft = null;
        //-Connections-//
        public static Connection gl_c_current = null;
        //-Integers-//
        public static int gl_i_numbOfSoft;
        //-Strings-//
        public static string gl_s_dateTimeOfSoftTransfere;
        //-Settings-//
        public string logFile = "server.log";
    }

    class ClientProgram
    {
        public static NotifyIcon TrayNotify = null;
        public static TimerCallback Upd = new TimerCallback(CheckServiceUpdate);
        public static _ClientThread _ClientActivities;
        static Thread _ClientProgramThread;
        static Thread _CrashMonitorThread;

        public static bool AllowToConnect = true;

        public static _ClientState CurrentClientStatus;
        public static LogType logLevel = LogType.Info;

        private static void ProcessExit(object sender, EventArgs e)
        {
            Process This = Process.GetCurrentProcess();
            This.Kill();
        }
        static void Main()
        {
            // Первоначальная настройка. Загрузка из реестра и прописывание некоторых параметров
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Установка трей иконки
            TrayNotify = new NotifyIcon()
            {
                Icon = Resources.net1,
                Visible = true,
                ContextMenuStrip = new ContextMenus().Create(),
                Text = "Сетевой клиент НИИ Телевидения"
            };
            
            CurrentClientStatus = _ClientState.Starting;
            
            Thread MyName = Thread.CurrentThread;
            if (MyName.Name == null)
                MyName.Name = "Основной поток";

            _CrashMonitorThread = new Thread(new ThreadStart(_СrashMonitor));
            _CrashMonitorThread.Name = "Поток монитора состояний";
            _CrashMonitorThread.Start();

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

            _ClientActivities = new _ClientThread();
            _ClientProgramThread = new Thread(new ThreadStart(_ClientActivities.Run));
            _ClientProgramThread.Name = "Новый Клиент (Main)";
            _ClientProgramThread.Start();
            
            // Старт таймеров и тредов
            Timer UpdateTimer = new Timer(Upd, null, 0, 3000000);

            while (CurrentClientStatus != _ClientState.Running) { if (CurrentClientStatus == _ClientState.Aborted) return; }
            TrayNotify.Icon = Resources.net2;
            logInfo("Все приготовления выполнены. Клиент запущен.");
            
            //
            Application.Run();
        }

        public static void _СrashMonitor()
        {
            while (true)
            {
                if (CurrentClientStatus == _ClientState.Aborted)
                {
                    logFatal("Критическая ошибка клиента. Перезапуск через 10 секунд.");
                    Thread.Sleep(10000);
                    RestartingClient();
                    break;
                }
                Thread.Sleep(2000);
            }
        }
        public static void StopClient()
        {
            AllowToConnect = false;
            gl_b_serverIsConnected = false;
            TrayNotify.Icon = Resources.net1;
            while (_ClientThread.connections.Count > 0)
            {
                // Waiting
            }

            if (_ClientProgramThread != null) _ClientProgramThread.Abort();
            Thread.Sleep(500);
        }

        public static void StartClient()
        {
            CurrentClientStatus = _ClientState.Starting;
            _ClientActivities = null;
            writeLog("", LogType.FileOnly);
            writeLog("-- Log Start [After Restart]: " + DateTime.Now + " --", LogType.FileOnly);
            _ClientActivities = new _ClientThread();
            AllowToConnect = true;
            _ClientProgramThread.Start();
            _CrashMonitorThread.Start();
            while (CurrentClientStatus != _ClientState.Running) { if (CurrentClientStatus == _ClientState.Aborted) return; }
            TrayNotify.Icon = Resources.net2;
            logInfo("Все приготовления выполнены. Клиент запущен.");
        }

        public static void RestartingClient()
        {
            CurrentClientStatus = _ClientState.Aborted;
            StopClient();
            logInfo("Поток завершился, перезапускаем.");
            Thread.Sleep(3000);
            StartClient();
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