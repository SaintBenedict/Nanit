using System;
using System.Collections.Generic;
using System.Text;

namespace NaNiT
{
    class XmlSoft : _XmlFile, IApplicationTransport
    {
        ServerObject MasterServer { get; set; }
        /// <summary>
        /// Массив/перечень под-свойств юзера
        /// </summary>
        public string[] childs { get; } = new string[] { "Version", "Publisher", "InstallDate", "InstallLocation" };
        
        /// <summary>
        /// Лист перечисляющий программы конкретного пользователя
        /// </summary>
        public List<_xApplication> List { get; set; }
        public List<_xApplication> ListOld { get; set; }
        /// <summary>
        /// XML юзер, работает с пользователями
        /// </summary>
        public XmlSoft(ServerObject server, string userFile) : base(@"ClientsBase\" + client.cryptoLogin + @"_Software.xml", "applications", "application")
        {
            xFilename = userFile;
            MasterServer = server;
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
            ServXUsers.SoftUp(MasterClient.cryptoLogin, DateTime.Now, List.Count, "no");
        }
    }
}
