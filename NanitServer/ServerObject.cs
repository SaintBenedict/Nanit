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

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            foreach (ClientObject clTemp in clients)
            {
                if (clTemp.Id == id)
                {
                    if (clTemp != null)
                    {
                        clients.Remove(clTemp);
                        clTemp.Close();
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
                Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, message);
                Globals.servState = true;

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
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
        protected internal void BroadcastMessage(string message, string id, List<ClientObject> clientsTemp, ClientObject clientOne, int whos)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            switch (whos)
            {
                case 0:
                    for (int i = 0; i < clientsTemp.Count; i++)
                    {
                        clients[i].Stream.Write(data, 0, data.Length); //передача данных
                    }
                    break;
                case 1:
                    clientOne.Stream.Write(data, 0, data.Length);
                    break;
            }
        }
        // отключение всех клиентов
        protected internal void Disconnect()
        {
            string dateStop = DateTime.Now.ToString();
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            if (Globals.servState)
            {
                string message = "Сервер прекратил работу: " + dateStop;
                Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, message);
                Globals.servState = false;
            }
        }
    }
}