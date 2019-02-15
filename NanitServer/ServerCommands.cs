using System;
using System.IO;
using System.Net.Sockets;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT
{
    class ServerCommands
    {
        public static int AwaitVarForCom = 0; // узнаём в каком кейсе ждать отвеа на наш псто
        public static int SoftwareMassive = 0; //Временная переменная с максимальным индексом софт массива
        public static string[] tempMass = new string[5]; //Массив из трёх (потому что в софт-массиве пока три ноды) наименований, чтобы получив их заполнить первый глобал-нод массива
        public static int checkMass = 0; // Но получать мы эти три будем поочереди и надо понять когда пора остановиться. тут то нам и поможет великолепный ЧЕКМАС
        public static int checkMuss = 0; // А на помощь ему придёт неподражаемый ЧЕКМУС!

        public static void CheckCommand(string message, ClientObject client, ServerObject server, NetworkStream Stream)
        {
            if (message == @"IamStupid-" || message.Length < 10)
            {
                client.dateLastSeen = DateTime.Now.ToString();
                gl_sList_Messages.Add(client.userName + " отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                server.BroadcastMessage(@"Fu(ck&&DI3-", ServerObject.clients, client, "self");
                server.RemoveConnection(client.Id);
                client.myMessageNotAwait = false;
                client.Close();
            }
            else
            {
                string command = message.Substring(0, 10);
                string textCom = message.Substring(10, message.Length - 10);
                switch (client.AwaitVarForCom)
                {

                    case 0: // Не ожидаем команды
                        switch (command)
                        {
                            default: // Дисконект из-за неверного сообщения
                                client.dateLastSeen = DateTime.Now.ToString();
                                gl_sList_Messages.Add(client.userName + " отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                                server.BroadcastMessage(@"Fu(ck&&DI3-", ServerObject.clients, client, "self");
                                server.RemoveConnection(client.Id);
                                client.myMessageNotAwait = false;
                                client.Close();
                                break;

                            case @"h@@lLloui-": // Клиент говорит Хэллоу, подключившись, мы спрашиваем кто он и ждём реги
                                client.userName = textCom;
                                gl_sList_Messages.Add(client.userName + " --- подключился");
                                // Проверка регистрации
                                server.BroadcastMessage("@HowdyHu%$-", ServerObject.clients, client, "self"); // спрашиваем кто он такой блин
                                client.AwaitVarForCom = 1;
                                client.myMessageNotAwait = true;
                                client.StupidCheck = false;
                                break;

                            case @"CH@T_AlL_-": // Команда админской рассылки мессейджа
                                SendChatToAll(textCom);
                                client.AwaitVarForCom = 0;
                                client.myMessageNotAwait = false;
                                client.StupidCheck = false;
                                break;

                            case @"SenDS0fT?-": // Божечки, да нас массивом софта завалят сейчас
                                client.AwaitVarForCom = 2;
                                client.myMessageNotAwait = true;
                                client.StupidCheck = false;
                                checkMuss = 0;
                                checkMass = 0;
                                gl_sList_Messages.Add(client.userName + " --- Начал передачу софт-массива [" + DateTime.Now.ToString() + "]");
                                SoftwareMassive = Convert.ToInt32(textCom);
                                File.Delete(@"ClientsBase\" + client.cryptoLogin + @"_Software.xml");
                                client.softXmlBase = new MyXml(@"ClientsBase\" + client.cryptoLogin + @"_Software.xml");
                                server.BroadcastMessage(@"OKb@@b1_g0-"+ checkMuss + @"%" + checkMass, ServerObject.clients, client, "self"); // говорим, что готовы въёбывать и отправляем по базе нулевой чекмас, намекая, что хотим его
                                break;
                        }
                        break;

                    case 1: // Ожидаем регистрацию
                        switch (command)
                        {
                            default: // Дисконект из-за неверного сообщения
                                client.dateLastSeen = DateTime.Now.ToString();
                                gl_sList_Messages.Add(client.userName + " --- отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                                server.BroadcastMessage(@"Fu(ck&&DI3-", ServerObject.clients, client, "self");
                                server.RemoveConnection(client.Id);
                                client.myMessageNotAwait = false;
                                client.Close();
                                break;

                            case @"R3GisSsTr-": // получаем регу и отвечаем, что она успешно прошла
                                client.cryptoLogin = textCom;
                                RegistrationOrLogin(textCom);
                                server.BroadcastMessage(@"1t$OKb@@b1-", ServerObject.clients, client, "self"); // успешная регистрация
                                client.AwaitVarForCom = 0;
                                client.myMessageNotAwait = true;
                                client.StupidCheck = false;
                                break;
                        }
                        break;

                    case 2: // Ждём лист софта
                        switch (command)
                        {
                            default: // Дисконект из-за неверного сообщения
                                client.dateLastSeen = DateTime.Now.ToString();
                                gl_sList_Messages.Add(client.userName + " --- отправил неверные данные и был отключён [" + client.dateLastSeen + "]");
                                server.BroadcastMessage(@"Fu(ck&&DI3-", ServerObject.clients, client, "self");
                                server.RemoveConnection(client.Id);
                                client.myMessageNotAwait = false;
                                client.Close();
                                break;

                            case @"SenD=Bi4h-": // Получаем софт-массив
                                tempMass[checkMass] = textCom;
                                checkMass++;
                                if (checkMass == 5)
                                {
                                    client.softXmlBase.AddApplication(tempMass[0], tempMass[1], tempMass[2], tempMass[3], tempMass[4]);
                                    checkMass = 0;
                                    checkMuss++;
                                    
                                }
                                server.BroadcastMessage(@"OKb@@b1_g0-" + checkMuss + @"%" + checkMass, ServerObject.clients, client, "self"); // Говорим, что получили и просим на этот раз уже первый чекмас
                                client.AwaitVarForCom = 2;
                                client.myMessageNotAwait = true;
                                client.StupidCheck = false;
                                break;

                            case @"SenD=LAST-": // "Это должен быть последний элемент. В противном случае "что-то пошло не так"
                                tempMass[4] = textCom;
                                client.softXmlBase.AddApplication(tempMass[0], tempMass[1], tempMass[2], tempMass[3], tempMass[4]);
                                client.softXmlBase.SaveThis();
                                server.BroadcastMessage(@"it$MaHBi4!-" + DateTime.Now.ToString(), ServerObject.clients, client, "self"); // Благодарим клиента за то, что он няшка и передаём время связи
                                gl_sList_Messages.Add(client.userName + " --- Закончил передачу софт-массива [" + DateTime.Now.ToString() + "]");
                                gl_xml.SoftUp(client, DateTime.Now, checkMuss);
                                client.AwaitVarForCom = 0;
                                client.myMessageNotAwait = true;
                                client.StupidCheck = false;
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }

            void RegistrationOrLogin(string name)
            {
                if (gl_xml.isRegistred(client))
                    gl_sList_Messages.Add(client.userName + " (" + client.IP + ") --- авторизовался в системе [" + DateTime.Now.ToString() + "]");
                else
                    gl_sList_Messages.Add(client.userName + " --- зарегистрировался в системе [" + client.dateOfRegister + "]");
            }

            void SendChatToAll(string textChat)
            {
                if (String.Format("{0}: {1}", client.userName, textChat) != String.Format("{0}: {1}", client.userName, null))
                {
                    textChat = String.Format("{0}: {1}", client.userName, textChat);
                    gl_sList_Messages.Add(textChat);
                    server.BroadcastMessage(textChat, ServerObject.clients, client, "all");
                }
            }
        }
    }
}
