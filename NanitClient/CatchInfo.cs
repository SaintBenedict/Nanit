using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using static NaNiT.GlobalVariable;

namespace NaNiT
{
    class CatchInfo
    {
        public static string[,,] dataResult = new string[50, 10, 2];
        public static void InfoGet()
        {
            DeleteExist(); /// Удаление имеющегося файла информации

            

            OutputSimple("Данные об Автоматизированном Рабочем Месте");

            OutputIniBlock("Данные о сетях");
            OutputSimple("• Имя компьютера: " + gl_s_myHostName);
            for (byte i = 0; i <= Dns.GetHostEntry(gl_s_myHostName).AddressList.Length - 1; i++)
            {
                if (Dns.GetHostEntry(gl_s_myHostName).AddressList[i].IsIPv6LinkLocal == false)
                {
                    string myIP = Dns.GetHostEntry(gl_s_myHostName).AddressList[i].ToString();
                    OutputSimple(myIP);
                }
            }

            OutputIniBlock("Операционная система");
            OutputResult("• Операционная система:", GetHardwareInfo("Win32_OperatingSystem", "Caption"), 1);
            OutputResult("Версия:", GetHardwareInfo("Win32_OperatingSystem", "Version"), 2);
            OutputResult("Сервис-пак:", GetHardwareInfo("Win32_OperatingSystem", "ServicePackMajorVersion"), 3);
            OutputResult("Серийный номер:", GetHardwareInfo("Win32_OperatingSystem", "SerialNumber"), 4);
            OutputResult("Дата установки:", GetHardwareInfo("Win32_OperatingSystem", "InstallDate"), 5);
            OutputSimple("");
            OutputResult("Рабочая группа:", GetHardwareInfo("Win32_ComputerSystem", "Domain"), 6);

            OutputIniBlock("Системные устройства");
            OutputResult("• Процессор:", GetHardwareInfo("Win32_Processor", "Name"), 7);
            OutputResult("Производитель:", GetHardwareInfo("Win32_Processor", "Manufacturer"), 8);
            OutputResult("Описание:", GetHardwareInfo("Win32_Processor", "Description"), 9);
            OutputSimple("");
            OutputResult("• Материнская плата:", GetHardwareInfo("Win32_BaseBoard", "Product"), 10);
            OutputResult("Производитель:", GetHardwareInfo("Win32_BaseBoard", "Manufacturer"), 11);
            OutputResult("Описание:", GetHardwareInfo("Win32_BaseBoard", "Description"), 12);
            OutputSimple("");
            OutputResult("• Оперативная память:", GetHardwareInfo("Win32_PhysicalMemory", "Manufacturer"), 13);
            OutputResult("Серийный номер:", GetHardwareInfo("Win32_PhysicalMemory", "SerialNumber"), 14);
            OutputResult("Объём (в байтах):", GetHardwareInfo("Win32_PhysicalMemory", "Capacity"), 15);
            OutputSimple("");
            OutputResult("• Видеокарта:", GetHardwareInfo("Win32_VideoController", "Name"), 16);
            OutputResult("Видеопроцессор:", GetHardwareInfo("Win32_VideoController", "VideoProcessor"), 17);
            OutputResult("Версия драйвера:", GetHardwareInfo("Win32_VideoController", "DriverVersion"), 18);
            OutputResult("Объем памяти (в байтах):", GetHardwareInfo("Win32_VideoController", "AdapterRAM"), 19);
            OutputSimple("");
            OutputResult("• Жесткий диск:", GetHardwareInfo("Win32_DiskDrive", "Caption"), 20);
            OutputResult("Интерфейс:", GetHardwareInfo("Win32_DiskDrive", "InterfaceType"), 21);
            OutputResult("Объем (в байтах):", GetHardwareInfo("Win32_DiskDrive", "Size"), 22);

            OutputIniBlock("Периферийные устройства");
            OutputResult("• Клавиатура:", GetHardwareInfo("Win32_Keyboard", "Description"), 23);
            OutputSimple("");
            OutputResult("• Мышь:", GetHardwareInfo("Win32_PointingDevice", "Description"), 24);
            OutputResult("Производитель:", GetHardwareInfo("Win32_PointingDevice", "Manufacturer"), 25);
            OutputSimple("");
            OutputResult("• Дисковод:", GetHardwareInfo("Win32_CDROMDrive", "Name"), 26);
            OutputResult("Буква привода:", GetHardwareInfo("Win32_CDROMDrive", "Drive"), 27);
            OutputSimple("");
            OutputResult("• Монитор:", GetHardwareInfo("Win32_DesktopMonitor", "DeviceID"), 28);

            OutputIniBlock("Установленные программы");
            OutputSimple(gl_sMas_programs[5, 0]);
            Process.Start(gl_s_nameFile);

            ///Application.Exit(); /* Кусок кода для версии со сбором данных и не более того */
            ///Process.GetCurrentProcess().Kill(); /*Кусок кода для версии со сбором данных и не более того */
            
            ///Console.ReadLine();
            ///
            /*string displayName;
            RegistryKey key;
            StreamWriter file = new StreamWriter(gl_s_nameFile, true);
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string + subkey.GetValue("InstallLocation") as string;
                //DisplayIcon
                file.WriteLine(displayName);
            }
            file.Dispose();*/
        }

        private static List<string> GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            List<string> result = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + WIN32_Class);
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj[ClassItemField] != null)
                        result.Add(obj[ClassItemField].ToString().Trim());
                    else
                        result.Add("Ошибка при получении параметра (NULL)");
                }
            }
            return result;
        }

        private static void OutputResult(string info, List<string> result, byte pNumber)
        {
            using (StreamWriter file = new StreamWriter(gl_s_nameFile, true))
            {
                if (info.Length > 0)
                    file.WriteLine(info);

                if (result.Count > 0)
                {
                    for (byte i = 0; i < result.Count; ++i)
                    {
                        file.WriteLine(result[i]);
                        //dataResult[pNumber, i, 0] = result[i];
                        //dataResult[pNumber, i, 1] = result.Count.ToString();
                        ///file.WriteLine(" DEBUG *номер запроса* "+ pNumber + " DEBUG *значение* " + dataResult[pNumber, i, 0] + " DEBUG *номер значения в листе* " + (i + 1) + " DEBUG *число значений в листе* " + dataResult[pNumber, i, 1]);
                    }
                }
            }
        }

        private static void OutputSimple(string name)
        {
            using (StreamWriter file = new StreamWriter(gl_s_nameFile, true))
            {
                file.WriteLine(name);
            }
        }

        private static void OutputIniBlock(string name)
        {
            using (StreamWriter file = new StreamWriter(gl_s_nameFile, true))
            {
                file.WriteLine("");
                file.WriteLine("*****[" + name + "]*****");
                file.WriteLine("");
            }
        }

        private static void DeleteExist()
        {
            string date2 = DateTime.Now.ToString();
            date2 = date2.Replace(":", "-");
            gl_s_nameFile = @date2 + @".txt";
            gl_s_nameFile = @".\" + gl_s_nameFile;
            if (File.Exists(gl_s_nameFile))
            {
                File.Delete(gl_s_nameFile);
            }
        }
    }
}
