﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NaNiT
{
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        public static List<ClientObject> clients = new List<ClientObject>(); // все подключения
        public ClientObject[] ClientsToClose;

        // добавление клиента
        protected internal void AddConnection(ClientObject clientObject)
        {
            try
            {
                clients.Add(clientObject);
            }
            catch (Exception ex) { MessageBox.Show("ServerObject(Add_cli) " + ex.Message); }
        }

        // удаление одного клиента
        protected internal void RemoveConnection(string id, string crypto, NetworkStream endStreamofThis, TcpClient lastCryOfYours)
        {
            foreach (ClientObject clTemp in clients)
            {
                try
                {
                    if (clTemp.Id == id || clTemp.cryptoLogin == crypto)
                    {
                        if (clTemp != null)
                        {
                            BroadcastMessage("Fu(ck&&DI3-", ServerObject.clients, clTemp, "self");
                            clients.Remove(clTemp);
                            Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, clTemp.userName + " отключился " + DateTime.Now.ToString());
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
                catch (Exception ex) { MessageBox.Show("ServerObject(Rem_cli) " + ex.Message); }
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
                    try
                    {
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        ClientObject clientObject = new ClientObject(tcpClient, this);
                        clientObject.myMessageNotAwait = false;
                        clientObject.CloseMePliz = false;
                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                    }
                    catch (Exception ex) { MessageBox.Show("ServerObject(Lis_Tcp_cli) " + ex.Message); }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ServerObject(Listen) " + ex.Message);
                Disconnect();
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, List<ClientObject> clientsTemp, ClientObject clientOne, string whos)
        {
            try
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
            catch (Exception ex) { MessageBox.Show("ServerObject(Brodc) " + ex.Message); }
        }

        // отключение всех клиентов
        protected internal void Disconnect()
        {
            bool CompletDel = false;
            if (!Globals.disconnectInProgress)
            {
                try
                {
                    Globals.disconnectInProgress = true;
                    Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, "Сервер прекратил работу: " + DateTime.Now.ToString());
                    Thread ClosedAll = new Thread(new ThreadStart(ClosedAllClients));
                    if (ClosedAll.Name == null)
                        ClosedAll.Name = "Close all clients";
                    ClosedAll.Start();
                    ClosedAll.Join();
                    if (CompletDel == true)
                    {
                        ClosedAll.Abort();
                        ClosedAll = null;
                        tcpListener.Stop(); //остановка сервера
                        tcpListener = null;
                        FormSOptions.listenThread.Abort();
                        FormSOptions.listenThread = null;
                        FormSOptions.server = null;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                catch (Exception ex) { MessageBox.Show("ServerObject(DCon_main) " + ex.Message); }
            }

            void ClosedAllClients()
            {
                if (clients != null)
                    while (clients.Count != 0)
                    {
                        try
                        {
                            foreach (ClientObject clTemp in clients)
                            {
                                if (clTemp != null)
                                {
                                    Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, clTemp.userName + " отключился " + DateTime.Now.ToString());
                                    clTemp.dateLastSeen = DateTime.Now.ToString();

                                    clTemp.Stream.Close();
                                    clTemp.Stream.Dispose();
                                    clTemp.client.Close();
                                    clTemp.client = null;
                                }
                            }
                            clients.Clear();
                        }
                        catch (Exception ex) { MessageBox.Show("ServerObject(DCon_close) " + ex.Message); }
                    }
                CompletDel = true;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}