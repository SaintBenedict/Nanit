using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT
{
    class Client
    {
        public static List<Connection> connections = new List<Connection>(); // все подключения
        public NetworkStream StreamOfApplication;
        TcpClient NewConnection;
        Thread newConnectThread;

        protected internal void Listen()
        {
            while (true)
            {
                if (connections.Count == 0)
                {
                    try
                    {
                        NewConnection = new TcpClient();
                        NewConnection.Connect(gl_s_servIP, gl_i_servPort); //подключение клиента
                        StreamOfApplication = NewConnection.GetStream(); // получаем поток
                        Connection ThisServerConnection = new Connection(NewConnection, this, StreamOfApplication);
                        gl_c_current = ThisServerConnection;
                        newConnectThread = new Thread(new ThreadStart(ThisServerConnection.Start));
                        newConnectThread.Name = "Новое соединение";
                        newConnectThread.Start();
                        Thread.Sleep(10000);
                    }
                    catch
                    {
                        Thread.Sleep(10000);
                    }
                }
                else
                {
                    if(!gl_b_serverIsConnected)
                    {
                        newConnectThread.Abort();
                        newConnectThread = null;
                        StreamOfApplication.Close();
                        StreamOfApplication = null;
                        NewConnection.Close();
                        NewConnection = null;
                        connections.Clear();
                        gl_s_serverStatus = "Сервер недоступен";
                        Program.notifyIcon.Icon = Resources.net1;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        Thread.Sleep(10000);
                    }
                    Thread.Sleep(2);
                }
            }
        }

        protected internal void AddConnection(Connection newConnect)
        {
            try
            {
                connections.Add(newConnect);
            }
            catch (Exception ex) { MessageBox.Show("Connection(Add_conn) " + ex.Message); }
        }

        protected internal void RemoveConnection(Connection ConnectionToDel)
        {
            try
            {
                
            }
            catch (Exception ex) { MessageBox.Show("Connection(Add_conn) " + ex.Message); }
        }
    }

    class Connection
    {
        protected internal TcpClient Tcp;
        Client ClientThis;
        protected internal NetworkStream StreamOfClient;

        public Connection(TcpClient NewTcp, Client ThisApplicationRun, NetworkStream thisNewStream)
        {
            Tcp = NewTcp;
            ClientThis = ThisApplicationRun;
            StreamOfClient = thisNewStream;
            gl_b_serverIsConnected = true;
            ClientThis.AddConnection(this);
        }

        // Первое подключение к серверу
        public void Start()
        {
            try
            {
                //client.Connect("127.0.0.1", 51780);
                BinaryReader sIn = new BinaryReader(StreamOfClient);
                BinaryWriter sOut = new BinaryWriter(StreamOfClient);
                MemoryStream packet = new MemoryStream();
                BinaryWriter packetWrite = new BinaryWriter(packet);
                byte[] buffer = Encoding.Unicode.GetBytes("Hopheylalaley");
                byte[] buffe2r = Encoding.Unicode.GetBytes("Pospero");
                byte[] buffe2r3 = Encoding.Unicode.GetBytes("Hopheylalaley" + "Pospero");
                packetWrite.Write((short)buffe2r3.Length);
                packetWrite.Write((short)buffer.Length);
                packetWrite.Write(buffe2r3);
                byte[] message = packet.ToArray();
                sOut.Write((short)7);
                sOut.Write(message.Length);
                sOut.Write(message);
                sOut.Flush();

                //string message = "h@@lLloui-" + gl_s_userName;
                //byte[] data = Encoding.Unicode.GetBytes(message);
                //StreamOfClient.Write(data, 0, data.Length);
                //// запускаем новый поток для получения данных
                gl_i_awaitVarForCom = 0;
                Program.notifyIcon.Icon = Resources.net2;
                gl_s_serverStatus = "Подключение установлено";
                while (Tcp != null)
                {
                    string MessageTestNull = ReceiveMessage();
                    if (MessageTestNull == null)
                    {
                        Disconnect();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                if (gl_b_serverIsConnected)
                    MessageBox.Show("Connection(start) " + ex.Message);
                else
                    Disconnect();
            }
            finally
            {
                if (gl_b_serverIsConnected)
                {
                    Disconnect();
                }
            }
        }

        // отправка сообщений
        public void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                StreamOfClient.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                if (!gl_b_serverIsConnected)
                    MessageBox.Show("Connection(Send) " + ex.Message);
                else
                    Disconnect();
            }
        }

        // получение сообщений
        public string ReceiveMessage()
        {
            if (StreamOfClient.CanRead)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        if (!gl_b_serverIsConnected)
                        {
                            Disconnect();
                            return null;
                        }
                        gl_b_myMessageNotAwait = true;
                        bytes = StreamOfClient.Read(data, 0, data.Length);
                        if (bytes == 0)
                            Disconnect();
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (StreamOfClient.DataAvailable);
                    FromServerCommands.DoWithServerCommand(builder.ToString(), this);
                    return builder.ToString();
                }
                catch (Exception ex)
                {
                    if (!gl_b_serverIsConnected)
                        MessageBox.Show("ClientObject(get_msg) " + ex.Message);
                    else
                        Disconnect();
                    return null;
                }

            }
            else
            {
                return null;
            }

        }

        // отключение
        public static void Disconnect()
        {
            if (gl_b_serverIsConnected)
            {
                gl_b_serverIsConnected = false;
            }
        }
    }
}

