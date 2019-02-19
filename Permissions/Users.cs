using System.IO;

namespace NaNiT.Permissions
{
    public class User
    {

        public string CryptoLogin;
        public string RegistredDate;
        public string LastSeenDate;
        public string HostShortName;
        public string UserIpAdress;
        public int DatabaseId;

        public User(string CryptoLogin, string RegistredDate, string LastSeenDate, string HostShortName, string UserIpAdress)
        {
            this.CryptoLogin = CryptoLogin;
            this.RegistredDate = RegistredDate;
            this.LastSeenDate = LastSeenDate;
            this.HostShortName = HostShortName;
            this.UserIpAdress = UserIpAdress;
        }
    }

    class Users
    {
        internal static string UsersPath { get { return Path.Combine(MainProgram.SavePath, "AutorisedClients"); } }

        public static void SetupUsers()
        {
            if (!Directory.Exists(UsersPath)) Directory.CreateDirectory(UsersPath);
        }
        
    }
}
