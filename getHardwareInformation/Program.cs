using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

/*Форматировать фрагмент кода - жмёшь Ctrl + K, отпускаешь и сразу жмёшь Ctrl + F.
Форматировать весь код - жмёшь Ctrl + K, отпускаешь и сразу жмёшь Ctrl + D*/

namespace NaNiT
{
    static class Globals
    {
        public static bool DEBUGMODE = true;
        public static string appVersion = "1.3.4"; // ЕТО НЕ НАСТОЯЩИЕ ЦИФРЫ, НЕ ЕШЬ ПОДУМОЙ
        public static string nanitSvcVer = "0";
        public static string version = Application.ProductVersion; /// Изменять в AssemblyInfo.cs версию, чтобы была такой же как ^^ app.Version
        public static string[] pathUpdate = new string[11];
        public static string nameFile = "";
        public static string optionsPasswordDefault = "478632";
        public static string optionsPasswordReg = "";
        public static string servIP = "127.0.0.1";
        public static string servPort = "51782";
        public static string md5PortIp = Functions.MD5Code(servPort + servIP);
        public static FormLogin form1 = null;
        public static FormOptions form2 = null;
        public static FormUpdater form3 = null;
        public static bool RoleSecurity = false;
        public static bool RoleMessager = false;
        public static bool RoleOperate = false;
        public static bool RoleAdmin = false;
        public static bool RoleAgent = true;
        public static string md5Clients = Functions.MD5Code(RoleSecurity.ToString().ToLower() + RoleMessager.ToString().ToLower() + RoleOperate.ToString().ToLower() + RoleAdmin.ToString().ToLower() + RoleAgent.ToString().ToLower());
        public static bool isAboutLoaded = false;
        public static bool isUpdOpen = false;
        public static bool isOptOpen = false;
        public static string exMessage = null;
        public static byte serviceStatus = 0; // Проверка службы обновлений. 0 не установлена и не запущена. 1 установлена и запущена. 2 установлена не запущена. 3 обновление. 4 обновление
        public static sbyte adrUpdNum = -1;
        public static string updVerAvi = "1.0.0"; // Стринг для версии файла доступного для обновления
        public static byte work = 30;
        public static byte workTmp = 30;
        public static int itemsInList = 0;
        public static int updateIn = 11;
    }
    class Program
    {
        public static NotifyIcon notifyIcon = null;

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Globals.form2 = new FormOptions();

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Properties.Resources.net2;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = new ContextMenus().Create();
            notifyIcon.Text = "Сетевой агент НИИ Телевидения";
            //Globals.pathUpdate[0] = @"http://mail.niitv.ru";
            Globals.appVersion = Globals.version;


            Functions.RegCheck();                       ///Проверка наличия настроек в реестре
                                                        ///Functions.InfoGet();                     ///Кусок кода для версии со сбором данных и не более того

            if (!Globals.DEBUGMODE)
                Functions.AutoSelfInstall();            ///Копирование себя в папку сервисов, запуск оттуда удаление лишнего

            Thread t = Thread.CurrentThread;
            t.Name = "Main Program";
            TimerCallback tm1 = new TimerCallback(CheckServiceUpdate);
            Application.Run();
            Timer timer1 = new Timer(tm1, 0, 0, 8000);
        }



        public static void CheckServiceUpdate(object obj)
        {
            Thread t2 = Thread.CurrentThread;
            t2.Name = "Ебучий таймер";
            //ServiceWork.CheckUpdServer();
            ServiceWork.ServiceInit();

            if (Globals.adrUpdNum != -1)
                if (Globals.nanitSvcVer != Globals.updVerAvi)
                {
                    //ServiceWork.UpdateService();
                }
        }
    }
}