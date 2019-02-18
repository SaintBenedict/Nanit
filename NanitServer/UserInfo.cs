using System;

namespace NaNiT
{
    public class UserInfo : IInformation
    {
        ClientObject Master { get; set; }
        public string UserHostName { get; set; }
        public string CryptedName { get; set; }
        public string DateLastOnline { get; set; }
        public string DateOfRegistration { get; set; }
        public string UserIpAdress { get; set; }
        public string Guid_id { get; set; }
        public int Database_id { get; set; }

        public UserInfo(ClientObject bigDaddy)
        {
            Master = bigDaddy;
            DateOfRegistration = null;
            DateLastOnline = DateTime.Now.ToString();
            Guid_id = Guid.NewGuid().ToString();

        }
    }
}