using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NaNiT
{
    class CFunc
    {
        static string userName = Globals.userName;
        static Thread receiveThread = null;
        static Thread ClosedAll = new Thread(new ThreadStart(ClosedAllClients));
        static Thread NullAll = new Thread(new ThreadStart(ClosedAllClients));

        //Подключение
        public static void Chat()
        {
            Program.client = new TcpClient();
            receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            if (receiveThread.Name == null)
                receiveThread.Name = "Receive Thread";
            try
            {
                Program.client.Connect(Globals.servIP, Globals.servPort); //подключение клиента
                Program.stream = Program.client.GetStream(); // получаем поток
                string message = "h@@lLloui-" + userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                Program.stream.Write(data, 0, data.Length);
                // запускаем новый поток для получения данных
                Globals.AwaitVarForCom = 0;
                receiveThread.Start();
                Program.notifyIcon.Icon = Properties.Resources.net3;
                Globals.serverIsConnected = true;
                Globals.TimConnLock = 0;
                Globals.serverStatus = "Подключение установлено";
            }
            catch (Exception ex)
            {
                MessageBox.Show("CFunc(chat) " + ex.Message);
            }
        }

        // отправка сообщений
        public static void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                Program.stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CFunc(Send) " + ex.Message);
            }
        }

        // получение сообщений
        public static void ReceiveMessage()
        {
            while (Globals.serverIsConnected)
            {
                if (Program.stream.CanRead)
                {
                    try
                    {
                        byte[] data = new byte[64]; // буфер для получаемых данных
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            Globals.myMessageNotAwait = true;
                            bytes = Program.stream.Read(data, 0, data.Length);
                            if (bytes == 0)
                                Disconnect();
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (Program.stream.DataAvailable);
                        FromServerCommands.DoWithServerCommand(builder.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("CFunc(Reciv) " + ex.Message);
                    }

                }

            }
            Disconnect();
        }

        // отключение
        public static void Disconnect()
        {
            if (!Globals.disconnectInProgress)
            {
                Globals.disconnectInProgress = true;
                
                if (CFunc.ClosedAll.Name == null)
                    CFunc.ClosedAll.Name = "Close all functions";
                
                if (CFunc.NullAll.Name == null)
                    CFunc.NullAll.Name = "Null all functions";

                CFunc.NullAll.Start();
                CFunc.ClosedAll.Start();

                
                
            }
        }
        private static void ClosedAllClients()
        {
            Program.client.Close();
            Program.stream.Close();
            
            if (Thread.CurrentThread.Name == "Null all functions")
            {
                while (Program.client != null && Program.stream != null)
                {
                    NullAll.Join();
                }
            }
            receiveThread.Abort();
            Program.stream.Dispose();
            Program.client = null;
            receiveThread = null;
            Globals.serverStatus = "Сервер стал недоступен";
            Program.notifyIcon.Icon = Properties.Resources.net2;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Globals.serverIsConnected = false;
        }
    }
}

