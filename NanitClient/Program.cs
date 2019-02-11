using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;
using static NaNiT.GlobalVariable;


namespace NaNiT
{
    class Program
    {
        public static NotifyIcon notifyIcon = null;
        public static TimerCallback Upd = new TimerCallback(CheckServiceUpdate);
        public static Thread ClientThread;

        static void Main()
        {
            // Первоначальная настройка. Загрузка из реестра и прописывание некоторых параметров
            Functions.FirstRunOptionsLoad();

            // Копирование себя в папку сервисов, запуск оттуда удаление лишнего
            Functions.AutoSelfInstall(false);

            // Старт соединения с сервером
            Client ThisExeStartClient = new Client();
            ClientThread = new Thread(new ThreadStart(ThisExeStartClient.Listen));
            if (ClientThread.Name == null)
                ClientThread.Name = "Новый Клиент (Main)";
            ClientThread.Start();

            // Старт таймеров и тредов
            Timer UpdateTimer = new Timer(Upd, null, 0, 3000000);

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

            if (gl_i_adrUpdNum != -1)
                if (gl_s_nanitSvcVer != gl_s_updVerAvi)
                {
                    ServiceWork.UpdateService();
                }
        }
    }
}