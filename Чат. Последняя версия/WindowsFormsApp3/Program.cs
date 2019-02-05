using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
///using System.Linq;
///using System.Threading.Tasks;
///using System.Management;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Security.Permissions;
using Microsoft.Win32;


namespace ChatClient
{
    static class Globals
    {
        public static СlientForm form1 = null;
        public static bool connect = false;

    }


    class Program
    {

        public static bool connect = false;
        static string userName;
        private const string host = "192.168.10.1";
        private const int port = 51782;
        static TcpClient client;
        static NetworkStream stream;




        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Globals.form1 = new СlientForm();
            Application.Run(Globals.form1);
        }

        //Подключение
        public static void Chat()
        {
            client = new TcpClient();
            string myHost = Dns.GetHostName();
            string myIP = null;
            for (int i = 0; i <= Dns.GetHostEntry(myHost).AddressList.Length - 1; i++)
            {
                if (Dns.GetHostEntry(myHost).AddressList[i].IsIPv6LinkLocal == false)
                {
                    myIP = Dns.GetHostEntry(myHost).AddressList[i].ToString();
                    userName = myIP;
                }
            }



            client = new TcpClient();
            client.Connect(host, port); //подключение клиента
            stream = client.GetStream(); // получаем поток
            string message = userName;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
            // запускаем новый поток для получения данных
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start(); //старт потока
        }
                                   // отправка сообщений
            public static void SendMessage()
            {
                string message = Globals.form1.textSend.Text;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

            }
            // получение сообщений
            public static void ReceiveMessage()
            {
                while (true)
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных

                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable); 
                    string message = builder.ToString();
                   // Globals.form1.textChat.Text = (Globals.form1.textChat.Text + @"\r\n" + message);//вывод сообщения 
                    Globals.form1.textChat.BeginInvoke((MethodInvoker)(delegate { Globals.form1.textChat.Text = Globals.form1.textChat.Text + Environment.NewLine + message; }));//вывод сообщения 
            }
            }

            public static void Disconnect()
            {
                string message = "ex";
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
                if (stream != null)
                    stream.Close();//отключение потока
                if (client != null)
                    client.Close();//отключение клиента
                connect = false;
                Environment.Exit(0); //завершение процесса
            }
        }
    }
