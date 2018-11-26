using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;

namespace getHardwareInformation
{
    class Program
    {
        private static Form1 form1 = null;
        public static NotifyIcon notifyIcon = null;
        public static string nameFile = "";

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.net2
            };
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = new ContextMenus().Create();
            notifyIcon.Text = "Сетевой агент НИИ Телевидения";


            Application.Run();
        }

        public static void InfoGet()
        {
            DeleteExist(); /// Удаление имеющегося файла информации

            string myHost = Dns.GetHostName();
            string myIP = null;

            OutputSimple("Данные об Автоматизированном Рабочем Месте");

            OutputIniBlock("Данные о сетях");
            OutputSimple("• Имя компьютера: "+myHost);

            for (int i = 0; i <= Dns.GetHostEntry(myHost).AddressList.Length - 1; i++)
            {
                if (Dns.GetHostEntry(myHost).AddressList[i].IsIPv6LinkLocal == false)
                {
                    myIP = Dns.GetHostEntry(myHost).AddressList[i].ToString();
                    OutputSimple(myIP);
                }
            }

            OutputIniBlock("Операционная система");
            OutputResult("• Операционная система:", GetHardwareInfo("Win32_OperatingSystem", "Caption"));
            OutputResult("Версия:", GetHardwareInfo("Win32_OperatingSystem", "Version"));
            OutputResult("Сервис-пак:", GetHardwareInfo("Win32_OperatingSystem", "ServicePackMajorVersion"));
            OutputResult("Серийный номер:", GetHardwareInfo("Win32_OperatingSystem", "SerialNumber"));
            OutputResult("Дата установки:", GetHardwareInfo("Win32_OperatingSystem", "InstallDate"));
            OutputSimple("");
            OutputResult("Рабочая группа:", GetHardwareInfo("Win32_ComputerSystem", "Domain"));

            OutputIniBlock("Системные устройства");
            OutputResult("• Процессор:", GetHardwareInfo("Win32_Processor", "Name"));
            OutputResult("Производитель:", GetHardwareInfo("Win32_Processor", "Manufacturer"));
            OutputResult("Описание:", GetHardwareInfo("Win32_Processor", "Description"));
            OutputSimple("");
            OutputResult("• Материнская плата:", GetHardwareInfo("Win32_BaseBoard", "Product"));
            OutputResult("Производитель:", GetHardwareInfo("Win32_BaseBoard", "Manufacturer"));
            OutputResult("Описание:", GetHardwareInfo("Win32_BaseBoard", "Description"));
            OutputSimple("");
            OutputResult("• Оперативная память:", GetHardwareInfo("Win32_PhysicalMemory", "Manufacturer"));
            OutputResult("Серийный номер:", GetHardwareInfo("Win32_PhysicalMemory", "SerialNumber"));
            OutputResult("Объём (в байтах):", GetHardwareInfo("Win32_PhysicalMemory", "Capacity"));
            OutputSimple("");
            OutputResult("• Видеокарта:", GetHardwareInfo("Win32_VideoController", "Name"));
            OutputResult("Видеопроцессор:", GetHardwareInfo("Win32_VideoController", "VideoProcessor"));
            OutputResult("Версия драйвера:", GetHardwareInfo("Win32_VideoController", "DriverVersion"));
            OutputResult("Объем памяти (в байтах):", GetHardwareInfo("Win32_VideoController", "AdapterRAM"));
            OutputSimple("");
            OutputResult("• Жесткий диск:", GetHardwareInfo("Win32_DiskDrive", "Caption"));
            OutputResult("Интерфейс:", GetHardwareInfo("Win32_DiskDrive", "InterfaceType"));
            OutputResult("Объем (в байтах):", GetHardwareInfo("Win32_DiskDrive", "Size"));

            OutputIniBlock("Периферийные устройства");
            OutputResult("• Клавиатура:", GetHardwareInfo("Win32_Keyboard", "Description"));
            OutputSimple("");
            OutputResult("• Мышь:", GetHardwareInfo("Win32_PointingDevice", "Description"));
            OutputResult("Производитель:", GetHardwareInfo("Win32_PointingDevice", "Manufacturer"));
            OutputSimple("");
            OutputResult("• Дисковод:", GetHardwareInfo("Win32_CDROMDrive", "Name"));
            OutputResult("Буква привода:", GetHardwareInfo("Win32_CDROMDrive", "Drive"));
            OutputSimple("");
            OutputResult("• Монитор:", GetHardwareInfo("Win32_DesktopMonitor", "DeviceID"));

            ///OutputIniBlock("Soft");
            ///OutputResult("OS:", GetHardwareInfo("Win32_OperatingSystem", "Name"));

            Process.Start(nameFile);
            /*
            ManagementObjectSearcher searcher_soft =
            new ManagementObjectSearcher("root\\CIMV2",
           "SELECT * FROM Win32_Product");

            foreach (ManagementObject queryObj in searcher_soft.Get())
            {
                Console.WriteLine("<soft> Caption: {0} ; InstallDate: {1}</soft>",
                                  queryObj["Caption"], queryObj["InstallDate"]);
            }*/
            ///Console.ReadLine();
        }

        private static List<string> GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            List<string> result = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + WIN32_Class);
            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    result.Add(obj[ClassItemField].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                ///Console.WriteLine(ex.Message);
            }

            return result;
        }

        private static void OutputResult(string info, List<string> result)
        {
            using (StreamWriter file = new StreamWriter(nameFile, true))
            {
                if (info.Length > 0)
                    file.WriteLine(info);

                if (result.Count > 0)
                {
                    for (int i = 0; i < result.Count; ++i)
                    {
                        file.WriteLine(result[i]);
                    }
                }
            }
        }

        private static void OutputSimple(string name)
        {
            using (StreamWriter file = new StreamWriter(nameFile, true))
            {
                file.WriteLine(name);
            }
        }

        private static void OutputIniBlock(string name)
        {
            using (StreamWriter file = new StreamWriter(nameFile, true))
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
            nameFile = @date2 + @".txt";
            nameFile = @".\" + nameFile;
            if (File.Exists(nameFile))
            {
                File.Delete(nameFile);
            }
        }

    }
}