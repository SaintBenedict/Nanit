using NaNiT.Functions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NaNiT.Permissions
{
    class ServerThread
    {
        public void Run()
        {
            TcpListener _listener = new TcpListener(IPAddress.Any, MainProgram.ServerConfig.serverPort);
            _listener.Start();

            MainProgram.logInfo("Сокет для подключений открыт");
            MainProgram.CurrentServerStatus = ServerState.Running;

            try
            {
                while (true)
                {
                    TcpClient clientSocket = _listener.AcceptTcpClient();
                    new Thread(new ThreadStart(new ClientThread(clientSocket).run)).Start();
                }
            }
            catch (Exception e)
            {
                MainProgram.logException("Поток прослушивания: " + e.ToString());
            }
            finally
            {
                _listener.Stop();
                MainProgram.logException("Прослушивающий поток закрылся, подключение новых клиентов невозможно.");
                MainProgram.CurrentServerStatus = ServerState.Crashed;
            }
        }
    }
}
