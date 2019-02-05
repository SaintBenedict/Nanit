using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace NaNiT
{
    class SFunctions
    {
        public static bool Revers(bool first)
        {
            if (first == true)
                return false;
            else
                return true;
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
        

        public static void RegCheck()
        {
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey localMachineSoftKey = localMachineKey.OpenSubKey("SOFTWARE", true);
            RegistryKey regNanit = localMachineSoftKey.CreateSubKey(@"N.A.N.I.T");
            RegistryKey servKey = regNanit.CreateSubKey("Update");
            
            Globals.servPort = Convert.ToInt32(CheckRegString("port_server", servKey, Globals.servPort.ToString()));

            string CheckRegString(string toRegName, RegistryKey toDo, string variant)
            {
                string result = variant;
                if (toDo.GetValue(toRegName) != null)
                    result = toDo.GetValue(toRegName).ToString();
                else
                    toDo.SetValue(toRegName, variant);
                return result;
            }

            regNanit.Close();
        }
    }
}
