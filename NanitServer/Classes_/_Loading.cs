using System;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT
{
    class _Loading
    {
        public static NotifyIcon TrayNotify = new NotifyIcon();
        public static System.Drawing.Icon Icon { get; set; }
        public static string TextOnShown { get; set; }

        public _Loading()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Установка Трей Иконки
            Tray(Resources.net3);
            // Проверка наличия настроек в реестре
            RegCheck();
            // Узнаём имя треда, для удобства дебага
            Thread MyName = Thread.CurrentThread;
            if (MyName.Name == null)
                MyName.Name = "Main Program";
            // Упрощённая версия загрузки, где окно сразу вылетает с логом
            gl_f_optionsServ = new FormSOptions();
            gl_f_optionsServ.Show();
            gl_b_isOptOpen = true;
            gl_c_server = new ServerObject();
        }

        public void ServerRestart()
        {

        }

        public static void Tray(System.Drawing.Icon resIcon)
        {
            Icon = resIcon;
            TextOnShown = "Сетевой сервер НИИ Телевидения";
            TrayNotify = new NotifyIcon();
            {
                TrayNotify.Icon = Icon;
                TrayNotify.Visible = true;
                TrayNotify.ContextMenuStrip = new SContextMenus().Create();
                TrayNotify.Text = TextOnShown;
            }
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