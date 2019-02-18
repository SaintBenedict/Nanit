using NaNiT.Functions;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NaNiT.Permissions
{
    class ServerThread
    {
        public void run()
        {
            TcpListener serversocket = new TcpListener(IPAddress.Any, MainProgram.config.serverPort);
            serversocket.Start();

            MainProgram.logInfo("Сокет для подключений открыт");
            MainProgram.serverState = ServerState.Running;

            try
            {
                while (true)
                {
                    TcpClient clientSocket = serversocket.AcceptTcpClient();
                    new Thread(new ThreadStart(new ClientThread(clientSocket).run)).Start();
                }
            }
            catch (Exception e)
            {
                MainProgram.logException("ListenerThread: " + e.ToString());
            }

            serversocket.Stop();
            MainProgram.logException("ListenerThread has failed - No new connections will be possible.");
            Console.ReadLine();
        }
    }
}
