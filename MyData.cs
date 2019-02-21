using System;

namespace NaNiT
{
    public class UserActiveInfo
    {
        public string CryptoLogin;
        public string RegistredDate;
        public string LastSeenDate;
        public string HostShortName;
        public string UserIpAdress;
        public int DatabaseId;

        public string client { get { if (String.IsNullOrEmpty(CryptoLogin)) return UserIpAdress; else return CryptoLogin; } }

        public UserActiveInfo(string crypto, string host)
        {
            this.CryptoLogin = crypto;
            this.HostShortName = host;
        }
    }
}
