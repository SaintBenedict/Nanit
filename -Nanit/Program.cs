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
            // Первоначальная настройка. Загрузка из реестра и прописывание некоторых параметров
            Functions.FirstRunOptionsLoad();

            // Копирование себя в папку сервисов, запуск оттуда удаление лишнего
            Functions.AutoSelfInstall(false);

            // Старт соединения с сервером
            CFunc.Chat();

            // Старт таймеров и тредов
            Timer UpdateTimer = new Timer(Upd, null, 0, 3000000);
            Thread ServStates = new Thread(new ThreadStart(TempServRun));
            ServStates.Start();

            //
            Application.Run();
        }


        public static void CheckServiceUpdate(object obj)
        {
            Thread MyThread = Thread.CurrentThread;
            if (MyThread.Name == null)
                MyThread.Name = "Update Timer";

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
            Thread ServRun = Thread.CurrentThread;
            if (ServRun.Name == null)
                ServRun.Name = "Server sender and reopener";
            do
            {
                while (!Globals.serverIsConnected)
                {
                    CFunc.Chat();
                    if (Globals.serverIsConnected)
                        break;
                    else
                        Thread.Sleep(10000);
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