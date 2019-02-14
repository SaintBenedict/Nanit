using System;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

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

            // Задание нулячего массива
            gl_xml = new _XmlUser();

            // Проверка наличия настроек в реестре
            RegCheck();

            // Узнаём имя треда, для удобства дебага
            Thread MyName = Thread.CurrentThread;
            if (MyName.Name == null)
                MyName.Name = "Main Program";


            gl_f_optionsServ = new FormSOptions();
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
