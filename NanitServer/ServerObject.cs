using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
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
                        clients.Remove(clTemp);
                    break;
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
                Globals.MessageIn = 1;
                Globals.MessageText = "Сервер запущен: " + dateStart;

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
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
               // if (clients[i].Id != id) // если id клиента не равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
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
            Globals.MessageIn = 3;
            Globals.MessageText = "Сервер прекратил работу: " + dateStop;
        }
    }
}