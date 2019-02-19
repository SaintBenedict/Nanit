using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;


namespace NaNiT
{
    public abstract class ServerApplication
    {
        public static FormSOptions ServerForm { get; set; }
        public static bool ServerFormIsOpen { get; set; } = false;
        public static bool TrayMenuIsOpen { get; set; } = false;
        public static NotifyIcon TrayNotify { get; set; }
        public static int ServerConnectionPort { get; set;}
        public static ServerObject MainServer { get; internal protected set; }
        public static Thread ListenThread { get; internal protected set; }
        public static List<string> LogMessageList = new List<string>();
        public static bool ServerIsDisconnecting { get; set; } = false;

        static void Maian()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ServerConnectionPort = 51782;
            RegCheck();
            
            TrayNotify = new NotifyIcon()
            {
                Icon = Resources.net4,
                Visible = true,
                ContextMenuStrip = new SContextMenus().Create(),
                Text = "Сетевой сервер НИИ Телевидения"
            };
            
            StartServer();

            ServerForm = new FormSOptions();
            ServerForm.Show();
            Application.Run();
        }

        
        public static void StartServer()
        {
            MainServer = new ServerObject();
            ListenThread = new Thread(new ThreadStart(MainServer.Starting));
            ThreadName("MainServer Listener", ListenThread);
            ListenThread.Start();

        }
        public static void RestartServer()
        {
            StopServer();
            Thread.Sleep(5000);
            StartServer();
        }
        public static void StopServer()
        {
            MainServer.Disconnect();
        }    
        public static void ThreadName(string _name, Thread _thread)
        {
            if (_thread.Name == null)
                _thread.Name = _name;
        }
        public static void ThreadName(string _name)
        {
            Thread _thisThread = Thread.CurrentThread;
            ThreadName(_name, _thisThread);
        }
        public static void RegCheck()
        {
            RegistryWork servReg = new RegistryWork("Server");
            ServerConnectionPort = servReg.ReadInt("port_server", ServerConnectionPort);
            servReg.Exit();
        }

    }
}