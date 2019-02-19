using NaNiT.Utils;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT.Permissions
{
    class AttemptingServer
    {
        public static List<Connection> connections = new List<Connection>(); // все подключения
        public NetworkStream StreamApp;
        internal TcpClient NewConnection;
        internal Thread newConnectThread;

        public void Run()
        {
            try
            {
                while (MainClient.StateMyActtivity != _ClientState.Crashing && MainClient.ServerStatus != ServChecker.Crashing)
                {
                    if (connections.Count == 0)
                    {
                        MainClient.StateMyActtivity = _ClientState.AtemtingToConnect;
                        NewConnection = new TcpClient();
                        NewConnection.Connect(gl_s_servIP, gl_i_servPort);
                        StreamApp = NewConnection.GetStream();
                        Connection ThisServerConnection = new Connection(NewConnection, this, StreamApp);
                        connections.Add(ThisServerConnection);
                        gl_c_current = ThisServerConnection;
                        MainClient.logInfo("Соединение с сервером установлено");
                        MainClient.StateMyActtivity = _ClientState.Connected;
                        MainClient.ServerStatus = ServChecker.IsConnecting;
                        MainClient.TrayNotify.Icon = Resources.net2;
                        newConnectThread = new Thread(new ThreadStart(ThisServerConnection.Start));
                        newConnectThread.Name = "Новое соединение";
                        newConnectThread.Start();
                        Thread.Sleep(10000);
                    }
                    else
                    {
                        if (MainClient.StateMyActtivity == _ClientState.Crashing || MainClient.StateMyActtivity == _ClientState.Disconnecting || MainClient.ServerStatus == ServChecker.Crashing || MainClient.ServerStatus == ServChecker.DisconnectingMe)
                        {
                            newConnectThread.Abort();
                            newConnectThread = null;
                            StreamApp.Close();
                            StreamApp = null;
                            NewConnection.Close();
                            NewConnection = null;
                            connections.Clear();
                            gl_s_serverStatus = "Сервер недоступен";
                            MainClient.StateMyActtivity = _ClientState.OfflineWork;
                            MainClient.TrayNotify.Icon = Resources.net1;
                            Thread.Sleep(10000);
                        }
                        Thread.Sleep(2);
                    }
                }
            }
            catch (Exception e)
            {
                MainClient.logException("Поток прослушивания: " + e.ToString());
            }
            finally
            {
                NewConnection.Close();
                MainClient.logException("Прослушивающий поток закрылся, подключение к серверу разорвано.");
                MainClient.StateMyActtivity = _ClientState.Disconnecting;
            }
        }
    }
}
