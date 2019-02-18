using System;
using System.Collections.Generic;
using System.Xml;

namespace NaNiT
{
    class XmlSoft : _XmlFile
    {
        ServerObject MainServer { get; set; }
        ClientObject ThisClient { get; set; }
        /// <summary>
        /// Массив/перечень под-свойств юзера
        /// </summary>
        public string[] Childs { get; } = new string[] { "Version", "Publisher", "InstallDate", "InstallLocation" };

        /// <summary>
        /// Лист перечисляющий программы конкретного пользователя
        /// </summary>
        public List<_xApplication> List { get; set; }
        public List<_xApplication> ListOld { get; set; }

        public _xApplication _xApplication { get; set; }

        /// <summary>
        /// XML юзер, работает с пользователями
        /// </summary>
        public XmlSoft(ServerObject server, ClientObject tempClient, string userFile) : base(@"ClientsBase\" + tempClient.MyInfo.CryptedName + @"_Software.xml", "applications", "application")
        {
            xFilename = userFile;
            MainServer = server;
            ThisClient = tempClient;
        }

        public void Add(_xApplication tempApplication)
        {
            this.List.Add(tempApplication);
        }

        public string[] AppMass(_xApplication xapp)
        {
            return new string[] { xapp.Name, xapp.Version, xapp.Publisher, xapp.InstallDate, xapp.InstallLocation };
        }

        public _xApplication AppObj(string[] Mass)   // _xApplication newProg = AppObj(tempMass)
        {
            return new _xApplication(this, Mass[0], Mass[1], Mass[2], Mass[3], Mass[4]);
        }

        public void AddNewUpdateNode()
        {
            MainServer.MyXmlUser.SoftUp(ThisClient.MyInfo.CryptedName, DateTime.Now, List.Count, "no");
        }
        

        // Добавление софта
        public void AddApplication(string Item, string Version, string Publisher, string Date, string Location)
        {
            // создаем новый элемент user
            XmlElement tempApp = xDoc.CreateElement("application");
            // создаем атрибут name
            XmlAttribute tempName = xDoc.CreateAttribute("name");
            // создаем элементы необходимые файлу
            XmlElement tempVer = xDoc.CreateElement("Version");
            XmlElement tempPublish = xDoc.CreateElement("Publisher");
            XmlElement tempInDate = xDoc.CreateElement("InstallDate");
            XmlElement tempInLoc = xDoc.CreateElement("InstallLocation");

            XmlText itemText = xDoc.CreateTextNode(Item);
            XmlText versText = xDoc.CreateTextNode(Version);
            XmlText publText = xDoc.CreateTextNode(Publisher);
            XmlText inDateText = xDoc.CreateTextNode(Date);
            XmlText inLocaText = xDoc.CreateTextNode(Location);
            tempName.AppendChild(itemText);
            tempVer.AppendChild(versText);
            tempPublish.AppendChild(publText);
            tempInDate.AppendChild(inDateText);
            tempInLoc.AppendChild(inLocaText);

            tempApp.Attributes.Append(tempName);
            tempApp.AppendChild(tempVer);
            tempApp.AppendChild(tempPublish);
            tempApp.AppendChild(tempInDate);
            tempApp.AppendChild(tempInLoc);
            xRoot.AppendChild(tempApp);
            Save();
        }
        
    }
}
