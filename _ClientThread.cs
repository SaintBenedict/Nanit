using NaNiT.Utils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT.Permissions
{
    class _ClientThread
    {
        public static List<Connection> connections = new List<Connection>(); // все подключения
        public NetworkStream StreamOfApplication;
        internal TcpClient NewConnection;
        internal Thread newConnectThread;

        public void Run()
        {
            try
            {
                while (true)
                {
                    if (connections.Count == 0 && ClientProgram.AllowToConnect)
                    {
                        NewConnection = new TcpClient();
                        NewConnection.Connect(gl_s_servIP, gl_i_servPort); //подключение клиента
                        StreamOfApplication = NewConnection.GetStream(); // получаем поток
                        Connection ThisServerConnection = new Connection(NewConnection, this, StreamOfApplication);
                        gl_c_current = ThisServerConnection;
                        ClientProgram.logInfo("Соединение с сервером установлено");
                        ClientProgram.CurrentClientStatus = _ClientState.Running;
                        newConnectThread = new Thread(new ThreadStart(ThisServerConnection.Start));
                        newConnectThread.Name = "Новое соединение";
                        newConnectThread.Start();
                        Thread.Sleep(10000);
                    }
                    else
                    {
                        if (!gl_b_serverIsConnected)
                        {
                            newConnectThread.Abort();
                            newConnectThread = null;
                            StreamOfApplication.Close();
                            StreamOfApplication = null;
                            NewConnection.Close();
                            NewConnection = null;
                            connections.Clear();
                            gl_s_serverStatus = "Сервер недоступен";
                            ClientProgram.TrayNotify.Icon = Resources.net1;
                            Thread.Sleep(10000);
                        }
                        Thread.Sleep(2);
                    }
                }
            }
            catch (Exception e)
            {
                ClientProgram.logException("Поток прослушивания: " + e.ToString());
            }
            finally
            {
                NewConnection.Close();
                ClientProgram.logException("Прослушивающий поток закрылся, подключение к серверу разорвано.");
                ClientProgram.CurrentClientStatus = _ClientState.Aborted;
            }
        }
    }
}
