
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace NaNiT
{
    class ServerCommands
    {
        public static int AwaitVarForCom = 0;
        public static void CheckCommand(string message, ClientObject client, ServerObject server, NetworkStream Stream)
        {
            switch (client.AwaitVarForCom)
            {
                case 0: // Не ожидаем команды
                    if (message.Length > 10)
                    {
                        string command = message.Substring(0, 10);
                        string textCom = message.Substring(10, message.Length-10);
                        switch (command)
                        {
                            default: // Просто рандомное сообщение
                                break;
                            case "R3GisSsTr-": // Команда регистрации или авторизации
                                RegistrationOrLogin(textCom);
                                client.AwaitVarForCom = 0;
                                break;
                            case "CH@T_AlL_-": // Команда админской рассылки мессейджа
                                SendChatToAll(textCom);
                                client.AwaitVarForCom = 0;
                                break;
                        }
                    }
                    break;
                case 1:
                    break;
                case 2:
                    break;
                default:
                    break;
            }



            void RegistrationOrLogin(string name)
            {
                string TempTime;
                string newMessage;
                foreach (string nameOfRegistred in Globals.AutorisedRegistredClients)
                {
                    if (client.userName == name)
                    {
                        if (client.userName == nameOfRegistred)
                        {
                            client.IsRegister = true;
                            if (client.dateLastSeen != null)
                                TempTime = client.dateLastSeen;
                            else
                                TempTime = "Не зафиксировано";
                            newMessage = client.userName + " авторизовался в системе " + DateTime.Now.ToString() + " Последнее появление " + TempTime;
                            Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, newMessage);
                            return;
                        }
                    }
                }
                if (client.userName == name)
                {
                    client.IsRegister = true;
                    Globals.AutorisedRegistredClients.Add(client.userName);
                    client.dateOfRegister = DateTime.Now.ToString();
                    newMessage = client.userName + " зарегистрировался в системе " + client.dateOfRegister;
                    Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, newMessage);
                }
            }
            void SendChatToAll(string textChat)
            {
                if (client.IsRegister)
                {
                    if (String.Format("{0}: {1}", client.userName, textChat) != String.Format("{0}: {1}", client.userName, null))
                    {

                        textChat = String.Format("{0}: {1}", client.userName, textChat);
                        Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, textChat);
                        server.BroadcastMessage(textChat, client.Id, ServerObject.clients, client, 0);
                    }
                    else
                    {
                        Stream.Close();
                        textChat = String.Format("{0}: отключился", client.userName);
                        Globals.MessageIn = SFunctions.ChangeMesIn(Globals.MessageIn, textChat);
                        textChat = null;
                        if (textChat != null)
                            server.BroadcastMessage(textChat, client.Id, ServerObject.clients, client, 0);
                    }
                }
            }
        }
    }
}
