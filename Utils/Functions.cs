using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NaNiT.Extensions;

namespace NaNiT.Functions
{
    public class Function
    {
        public static string ByteArrayToString(byte[] buffer)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (byte b in buffer)
                sb.Append(b.ToString("X2"));

            return (sb.ToString());
        }

        public static string ByteToBinaryString(byte byteIn)
        {
            StringBuilder out_string = new StringBuilder();
            byte mask = 128;
            for (int i = 7; i >= 0; --i)
            {
                out_string.Append((byteIn & mask) != 0 ? "1" : "0");
                mask >>= 1;
            }
            return out_string.ToString();
        }

        public static string StarHashPassword(string message, string salt, int rounds)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] messageBuffer = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
            byte[] saltBuffer = Encoding.UTF8.GetBytes(salt);

            while (rounds > 0)
            {
                MemoryStream ms = new MemoryStream();
                ms.Write(messageBuffer, 0, messageBuffer.Length);
                ms.Write(saltBuffer, 0, saltBuffer.Length);
                messageBuffer = sha256.ComputeHash(ms.ToArray());
                rounds--;
            }

            return Convert.ToBase64String(messageBuffer);
        }

        public static string GenerateSecureSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[24];
            rng.GetNonZeroBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        public static int getTimestamp()
        {
            int unixTimeStamp;
            DateTime currentTime = DateTime.Now;
            DateTime zuluTime = currentTime.ToUniversalTime();
            DateTime unixEpoch = new DateTime(1970, 1, 1);
            unixTimeStamp = (Int32)(zuluTime.Subtract(unixEpoch)).TotalSeconds;
            return unixTimeStamp;
        }
        
    }
}
