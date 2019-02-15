using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NaNiT
{
    public class _ClientValues
    {
        public string UserHostName { get; set; }
        public string CryptedName { get; set; }
        public string DateLastOnline { get; set; }
        public string DateOfRegistration { get; set; }
        public int Database_id { get; set; }
        public string UserIpAdress { get; set; }
        public string Guid_id { get; set; }

        public _ClientValues(TcpClient tcpClient, ServerObject serverObject)
        {
            DateLastOnline = DateTime.Now.ToString();
            Guid_id = Guid.NewGuid().ToString();
        }
    }
}