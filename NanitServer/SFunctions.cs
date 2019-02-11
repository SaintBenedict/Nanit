using System;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;

/*Форматировать фрагмент кода - жмёшь Ctrl + K, отпускаешь и сразу жмёшь Ctrl + F.
Форматировать весь код - жмёшь Ctrl + K, отпускаешь и сразу жмёшь Ctrl + D*/


namespace NaNiT
{
    class SFunctions
    {
        public static void FirstRunOptionsLoad()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Установка трей иконки
            SProgram.notifyIcon = new NotifyIcon();
            SProgram.notifyIcon.Icon = Resources.net3;
            SProgram.notifyIcon.Visible = true;
            SProgram.notifyIcon.ContextMenuStrip = new SContextMenus().Create();
            SProgram.notifyIcon.Text = "Сетевой сервер НИИ Телевидения";

            // Проверка наличия настроек в реестре
            RegCheck();

            // Узнаём имя треда, для удобства дебага
            Thread MyName = Thread.CurrentThread;
            if (MyName.Name == null)
                MyName.Name = "Main Program";


            gl_f_optionsServ = new FormSOptions();
        }
       

        public static int ChangeMesIn(int first, string message)
        {
            gl_sList_Messages.Add(message);
            if (first == 500)
                return 501;
            else
                return 500;
        }

        public static void RegCheck()
        {
            try
            {
                RegistryWork servReg = new RegistryWork("Server");
                gl_i_servPort = servReg.ReadInt("port_server", gl_i_servPort);
                servReg.Exit();
            }
            catch (Exception ex) { MessageBox.Show("SFunctions(RegCh) " + ex.Message); }
        }
    }
}
