﻿using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;


namespace NaNiT
{
    class LocalGlobals
    {
        //-Forms-//
        public static FormSOptions gl_f_optionsServ = null;
        //-Connections-//
        public static ServerObject gl_c_server = null;
        //-Strings-//
        public static string[] gl_sMas_nameNodes;
    }

    class SProgram
    {
        public static NotifyIcon notifyIcon = null;

        static void Main()
        {
            // Первоначальная настройка. Загрузка из реестра и прописывание некоторых параметров
            SFunctions.FirstRunOptionsLoad();

            // Упрощённая версия загрузки, где окно сразу вылетает с логом
            gl_f_optionsServ.Show();
            gl_b_isOptOpen = true;

            // Старт сервера
            FormSOptions.Start();

            // Старт таймеров и тредов
            /*Timer UpdateTimer = new Timer(Upd, null, 0, 3000000);
            Thread ServStates = new Thread(new ThreadStart(TempServRun));
            ServStates.Start();*/
            MyXml ServerSaveUsers = new MyXml("RegistredUsers");
            //
            Application.Run();

        }
    }
}