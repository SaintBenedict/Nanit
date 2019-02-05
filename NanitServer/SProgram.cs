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
        public static int servPort = 51782;
        public static FormSOptions form1 = null;
        public static bool isAboutLoaded = false;
        public static bool isOptOpen = false;
    }
    class SProgram
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
            notifyIcon.ContextMenuStrip = new SContextMenus().Create();
            notifyIcon.Text = "Сетевой сервер НИИ Телевидения";


            SFunctions.RegCheck();                       ///Проверка наличия настроек в реестре           ///Копирование себя в папку сервисов, запуск оттуда удаление лишнего

            Thread t = Thread.CurrentThread;
            t.Name = "Main Program";
            //TimerCallback tm1 = new TimerCallback(CheckServiceUpdate);
            //Timer timer1 = new Timer(tm1, 0, 0, 300000);
            Application.Run();
        }



        public static void CheckServiceUpdate(object obj)
        {
            Thread t2 = Thread.CurrentThread;
            t2.Name = "Ебучий таймер";
        }
    }
}