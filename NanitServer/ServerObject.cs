using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NaNiT
{
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        public static List<ClientObject> clients = new List<ClientObject>(); // все подключения
        public ClientObject[] ClientsToClose;

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id, string name, NetworkStream endStreamofThis, TcpClient lastCryOfYours)
        {
            foreach (ClientObject clTemp in clients)
            {
                if (clTemp.Id == id)
                {
                    if (clTemp != null)
                    {
                        clients.Remove(clTemp);
                        Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, name + " отключился " + DateTime.Now.ToString());
                        if (endStreamofThis != null) { endStreamofThis.Close(); }
                        if (lastCryOfYours != null) { lastCryOfYours.Close(); lastCryOfYours = null; }
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                string dateStart = DateTime.Now.ToString();
                tcpListener = new TcpListener(IPAddress.Any, Globals.servPort);
                tcpListener.Start();
                string message = "Сервер запущен: " + dateStart;
                Globals.disconnectInProgress = false;
                Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, message);

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    clientObject.myMessageNotAwait = false;
                    clientObject.CloseMePliz = false;
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch
            {
                Disconnect();
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, string id, List<ClientObject> clientsTemp, ClientObject clientOne, string whos)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            switch (whos)
            {
                case "all":
                    for (int i = 0; i < clientsTemp.Count; i++)
                    {
                        clients[i].Stream.Write(data, 0, data.Length); //передача данных
                    }
                    break;
                case "self":
                    clientOne.Stream.Write(data, 0, data.Length);
                    break;
            }
        }
        // отключение всех клиентов
        protected internal void Disconnect()
        {
            bool CompletDel = false;
            if (!Globals.disconnectInProgress)
            {
                Globals.disconnectInProgress = true;
                Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, "Сервер прекратил работу: " + DateTime.Now.ToString());
                Thread ClosedAll = new Thread(new ThreadStart(ClosedAllClients));
                ClosedAll.Name = "Close all clients";
                ClosedAll.Start();

                void ClosedAllClients()
                {
                    while (clients.Count != 0)
                    {
                        foreach (ClientObject clTemp in clients)
                        {
                            if (clTemp != null)
                            {
                                Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, clTemp.userName + " отключился " + DateTime.Now.ToString());
                                clTemp.dateLastSeen = DateTime.Now.ToString();
                                clTemp.Stream.Close();
                                clTemp.client.Close();
                                clTemp.client = null;
                            }
                        }
                        clients.Clear();
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    CompletDel = true;
                }

                ClosedAll.Join();
                if (CompletDel == true)
                {
                    tcpListener.Stop(); //остановка сервера
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }
    }
}