using System;
using System.Text.RegularExpressions;
using System.Threading;
using static NaNiT.GlobalVariable;
using static NaNiT.LocalGlobals;

namespace NaNiT
{
    class FromServerCommands
    {
        public static void DoWithServerCommand(string message, Connection current)
        {
            int tempNumbProg = 0;
            int variablesSoft = 0;
            
            if (message == @"Fu(ck&&DI3-" || message.Length < 11)
            {
                gl_s_serverStatus = "Сервер послал сигнал отключения";
                Program.notifyIcon.Icon = Resources.net1;
                gl_b_serverIsConnected = false;
                gl_i_awaitVarForCom = 0;
                gl_b_myMessageNotAwait = true;
            }
            else
            {
                string command = message.Substring(0, 11);
                string textCom = message.Substring(11, message.Length - 11);
                switch (gl_i_awaitVarForCom)
                {
                    case 0: // Ожидаем запрос на регистрацию
                        switch (command)
                        {
                            default: // Ошибочное сообщение
                                current.SendMessage(@"IamStupid-");
                                gl_i_awaitVarForCom = 0;
                                gl_b_myMessageNotAwait = true;
                                break;
                            case @"@HowdyHu%$-": // Получаем запрос от сервера
                                current.SendMessage(@"R3GisSsTr-" + gl_s_userName); // И отвечаем на него рег-мессейджем
                                gl_i_awaitVarForCom = 1;
                                gl_b_myMessageNotAwait = false;
                                break;
                        }
                        break;

                    case 1: // Ожидаем регистрацию
                        switch (command)
                        {
                            default: // Ошибочное сообщение
                                current.SendMessage(@"IamStupid-");
                                gl_i_awaitVarForCom = 0;
                                gl_b_myMessageNotAwait = true;
                                break;
                            case @"1t$OKb@@b1-": // Пришло подтверждение успешной регистрации, можно начинать обмениваться данными
                                gl_i_awaitVarForCom = 0;
                                gl_b_myMessageNotAwait = true;
                                Thread SendDatas = new Thread(new ThreadStart(SendDatasThread));
                                if (SendDatas.Name == null)
                                    SendDatas.Name = "Тред для дата-сэнда";
                                SendDatas.Start();
                                break;
                        }
                        break;

                    case 2: // Отправили инфу о софте, ждём респонса.
                        switch (command)
                        {
                            default: // Ошибочное сообщение
                                current.SendMessage(@"IamStupid-");
                                gl_i_awaitVarForCom = 0;
                                gl_b_myMessageNotAwait = true;
                                break;

                            case @"OKb@@b1_g0-": // Сервер готов ПРИНИМАТЬ от нас софт массив
                                string input = textCom;
                                string pattern = @"(%)";
                                string[] substrings = Regex.Split(input, pattern);
                                tempNumbProg = Convert.ToInt32(substrings[0]);
                                variablesSoft = Convert.ToInt32(substrings[2]);
                                if (tempNumbProg == gl_i_numbOfSoft && variablesSoft == 4)
                                {
                                    current.SendMessage(@"SenD=LAST-" + gl_sMas_programs[tempNumbProg, variablesSoft]);
                                }
                                else
                                    current.SendMessage(@"SenD=Bi4h-" + gl_sMas_programs[tempNumbProg, variablesSoft]); // а мы и не против
                                gl_i_awaitVarForCom = 2;
                                gl_b_myMessageNotAwait = false;
                                break;

                            case @"it$MaHBi4!-": // Нам спасибушки и время передачи данных
                                gl_s_dateTimeOfSoftTransfere = textCom;
                                gl_i_awaitVarForCom = 0;
                                gl_b_myMessageNotAwait = true;
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }

            void SendDatasThread()
            {
                //Создание объекта для генерации чисел
                Random rnd = new Random();
                //Получить случайное число (в диапазоне от 0 до 10)
                int value = rnd.Next(0, 10);
                Thread.Sleep(value * 10000); // Но обмениваться данными, мы будем не сразу, а через рандомное время, чтобы не перегрузить сервер
                if (gl_b_serverIsConnected)
                {
                    //First//
                    current.SendMessage(@"SenDS0fT?-" + gl_i_numbOfSoft); // Посылаем количество наименований в софт-массиве
                    gl_i_awaitVarForCom = 2;
                    gl_b_myMessageNotAwait = false;
                }

                /*gl_sMas_programs[i, 0] = listView1.Items[i].SubItems[0].Text;
                gl_sMas_programs[i, 1] = listView1.Items[i].SubItems[1].Text;
                gl_sMas_programs[i, 2] = listView1.Items[i].SubItems[2].Text;*/
            }
        }
    }
}