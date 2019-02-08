using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NaNiT
{
    class CFunc
    {
        static string userName = Globals.userName;

        //
        //Подключение
        public static void Chat()
        {
            Program.client = new TcpClient();
            try
            {
                Program.client.Connect(Globals.servIP, Globals.servPort); //подключение клиента
                Program.stream = Program.client.GetStream(); // получаем поток
                string message = "h@@lLloui-" + userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                Program.stream.Write(data, 0, data.Length);
                // запускаем новый поток для получения данных
                Globals.AwaitVarForCom = 0;
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                Globals.serverIsConnected = true;
                Globals.TimConnLock = 0;
                Globals.serverStatus = "Подключение установлено";
                Thread.Sleep(100);
            }
            catch
            {
                Disconnect();
                return;
            }
        }

        //
        // отправка сообщений
        public static void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            Program.stream.Write(data, 0, data.Length);
        }

        //
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
                            bytes = Program.stream.Read(data, 0, data.Length);
                            Globals.myMessageNotAwait = true;
                            if (bytes == 0)
                                Disconnect();
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (Program.stream.DataAvailable);
                        FromServerCommands.DoWithServerCommand(builder.ToString());
                    }
                    catch
                    {
                        Disconnect();
                    }

                }
                else
                {
                    Disconnect();
                }
            }
            Disconnect();
        }

        //
        // отключение
        public static void Disconnect()
        {
            if (!Globals.disconnectInProgress)
            {
                Globals.disconnectInProgress = true;
                if (Program.stream != null)
                    Program.stream.Close(); //отключение потока
                if (Program.client != null)
                    Program.client.Close(); //отключение клиента
                Globals.serverStatus = "Сервер стал недоступен";
                Globals.serverIsConnected = false;
            }
        }
    }
}

