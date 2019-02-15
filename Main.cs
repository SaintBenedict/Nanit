using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static NaNiT.GlobalVariable;

namespace NaNiT
{

    public class GlobalVariable
    {
        //-Bools-//
        public static bool gl_b_debug = true; // Если "Истина", то включены многие проверочные элементы. Только для дебагинга.
        public static bool gl_b_roleSecurity = false, gl_b_roleMessager = false, gl_b_roleOperate = false, gl_b_roleAdmin = false, gl_b_roleAgent = true; // Доступные клиенту Роли
        public static bool gl_b_serverIsConnected = false, gl_b_myMessageNotAwait = false, gl_b_disconnectInProgress = false, gl_b_tempSwitch = true; // Переменные статуса Сервера
        public static bool gl_b_isAboutLoaded = false, gl_b_isUpdOpen = false, gl_b_isOptOpen = false, gl_b_isOptOpenStatic = true, gl_b_isSoftOpen = false; // Проверка открытости форм
        public static bool gl_b_serviceInitLock = false, gl_b_installLock = false, gl_b_updateLock = false, gl_b_workLock = true; // Локеры функций
        ///
        ///
        //-Strings-//
        public static string gl_s_OSdate, gl_s_OSdateCrypt, gl_s_myHostName; // Получение даты установки ОС и имени компьютера
        public static string gl_s_nanitSvcVer = "0", gl_s_updVerAvi = "1.0.0", gl_s_version = Application.ProductVersion; // Версии
        public static string gl_s_optionsPasswordDefault = "478632", gl_s_servIP = "127.0.0.1"; // Стандартный пароль и адресс сервера
        public static string gl_s_md5PortIp, gl_s_md5Clients, gl_s_optionsPasswordReg; // Изменение переменных после их прогона через MD5
        public static string gl_s_userName, gl_s_serverStatus, gl_s_nameFile; // Переменный для общения с сервером, выгрузки и прочее
        ///
        public static string[,] gl_sMas_programs = null; // Массив со списком программ
        public static string[] gl_sMas_pathUpdate = new string[11]; // Массив со списком адресов обновлений
        ///
        public static List<string> gl_sList_autorisedRegistredClients = new List<string>(); // Лист авторизованных клиентов
        public static List<string> gl_sList_Messages = new List<string>(); // Лист авторизованных клиентов
        ///
        ///
        //-Integers-//
        public static int gl_i_adrUpdNum = -1, gl_i_itemsInList = 0, gl_i_updateIn = 11, gl_i_timerConnLock = 0; // Переменные для обновлений
        public static int gl_i_servPort = 51782, gl_i_awaitVarForCom = 0; // Связь с сервером
        ///
        ///
        //-Bytes-//
        public static byte gl_by_serviceStatus = 0; // Статус сервиса обновлений
    }


    public class GlobalFunctions
    {
        public static string GetOSDate()
        {
            string result = null, result1 = null, result2 = null, result3 = null;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + "Win32_OperatingSystem");
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["InstallDate"] != null)
                        result = obj["InstallDate"].ToString().Trim();
                }
            }
            result = result.Substring(0, 14);
            result1 = (MD5Code(result)).Substring(0, 6);
            result2 = (MD5Code(result)).Substring(9, 5);
            result3 = result1 + @"#" + result.Substring(2, 2) + @"_" + result.Substring(4, 4) + @"#" + result2 + "-" + (MD5Code(result)).Substring(16, 2);
            gl_s_OSdateCrypt = result3;
            return result;
        }

        public static string MD5Code(string getCode)
        {
            byte[] hash = Encoding.ASCII.GetBytes(getCode);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashenc = md5.ComputeHash(hash);
            string result = "";
            foreach (var b in hashenc)
            {
                result += b.ToString("x2");
            }
            return result;
        }

        public static bool Revers(bool first)
        {
            if (first == true)
                return false;
            else
                return true;
        }
    }

    public class RegistryWork
    {
        protected internal RegistryKey SubKey;
        protected internal string sSubKey = null;
        protected internal RegistryKey regWorkLocal;
        protected internal RegistryKey regWorkNanit;

        public RegistryWork(string newSub = @"N.A.N.I.T")
        {
            regWorkLocal = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            if (newSub != @"N.A.N.I.T")
            {
                regWorkNanit = regWorkLocal.CreateSubKey(@"N.A.N.I.T");
                sSubKey = newSub;
                SubKey = regWorkNanit.CreateSubKey(newSub);
            }
            else
            {
                SubKey = regWorkLocal.CreateSubKey(@"N.A.N.I.T");
            }
        }

        public void Exit()
        {
            SubKey.Close();
            SubKey = null;
            if (sSubKey != null)
            {
                regWorkNanit.Close();
                regWorkNanit = null;
                sSubKey = null;
            }
            regWorkLocal.Close();
            regWorkLocal = null;
        }
        
        public string ReadString(string From, string VarTo)
        {
            string resultStr = VarTo;
            if (SubKey.GetValue(From) != null)
                resultStr = SubKey.GetValue(From).ToString();
            else
                SubKey.SetValue(From, VarTo);
            return resultStr;
        }
        public bool ReadBool(string From, bool BoolTo)
        {
            bool resultBool = BoolTo;
            if (SubKey.GetValue(From) != null)
                resultBool = SubKey.GetValue(From).Equals("true");
            else
                SubKey.SetValue(From, BoolTo.ToString().ToLower());
            return resultBool;
        }
        public int ReadInt(string From, int IntTo)
        {
            int resultInt = IntTo;
            if (SubKey.GetValue(From) != null)
                resultInt = Convert.ToInt32(SubKey.GetValue(From));
            else
                SubKey.SetValue(From, IntTo.ToString());
            return resultInt;
        }

        public void Write(string RegTo, object From)
        {
            Type TypeTo = From.GetType();
            string type = TypeTo.Name;
            switch(type)
            {
                case "String":
                    SubKey.SetValue(RegTo, From);
                    break;
                case "Boolean":
                    SubKey.SetValue(RegTo, From.Equals("true"));
                    break;
                default:
                    SubKey.SetValue(RegTo, From.ToString());
                    break;
            }
        }
        
    }
}