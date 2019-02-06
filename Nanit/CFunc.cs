using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NaNiT
{
    class CFunc
    {
        static string userName = Globals.userName;
        static private string host = Globals.servIP;
        static private int port = Globals.servPort;

        //
        //Подключение
        public static void Chat()
        {
            Program.client = new TcpClient();
            try
            {
                Program.client.Connect(host, port); //подключение клиента
                Program.stream = Program.client.GetStream(); // получаем поток
                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                Program.stream.Write(data, 0, data.Length);
                // запускаем новый поток для получения данных
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
                            if (bytes == 0)
                            {
                                Disconnect();
                            }
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                            DoWithServerCommand(builder.ToString());
                        }
                        while (Program.stream.DataAvailable);
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
            if (Program.stream != null)
                Program.stream.Close(); //отключение потока
            if (Program.client != null)
                Program.client.Close(); //отключение клиента
            Globals.serverStatus = "Сервер стал недоступен";
            Globals.serverIsConnected = false;
            Globals.TimConnLock = Functions.ChangeMesIn(Globals.TimConnLock);
            Program.TempServConnect(Globals.TimConnLock);
        }

        public static void DoWithServerCommand(string sent)
        {
            if (sent.Length > 10 && sent.Substring(0, 11) == @"@HowdyHu%$-")
            {
                string command = sent.Substring(0, 11);
                string textCom = sent.Substring(11, sent.Length - 11);

                switch (command)
                {
                    default: // Просто рандомное сообщение
                        break;
                    case "@HowdyHu%$-": // Команда регистрации или авторизации
                        SendMessage(@"R3GisSsTr-" + Globals.userName);
                        break;
                }
            }
        }
    }
}

