using System;
using System.IO;
using System.Text;

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

        public static int getTimestamp()
        {
            int unixTimeStamp;
            DateTime currentTime = DateTime.Now;
            DateTime zuluTime = currentTime.ToUniversalTime();
            DateTime unixEpoch = new DateTime(1970, 1, 1);
            unixTimeStamp = (Int32)(zuluTime.Subtract(unixEpoch)).TotalSeconds;
            return unixTimeStamp;
        }
        
        public static void WriteToString(short _id, BinaryWriter _bw, string _message)
        {
            MemoryStream packet = new MemoryStream();
            BinaryWriter packetWrite = new BinaryWriter(packet);
            byte[] buffer = Encoding.Unicode.GetBytes(_message);
            packetWrite.Write((short)buffer.Length);
            packetWrite.Write(buffer);
            byte[] message = packet.ToArray();
            _bw.Write((short)7);
            _bw.Write(message.Length);
            _bw.Write(message);
            _bw.Flush();
        }

        public static string ReadString(BinaryReader _packetData)
        {
            short len = _packetData.ReadInt16();
            byte[] data = new byte[len]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            bytes = _packetData.Read(data, 0, len);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            string _result = builder.ToString();
            return _result;
        }
        public static string[] ReadStrings(BinaryReader _packetData, int _val)
        {
            short[] len = new short[_val];
            int[] bytes = new int[_val];
            string[] result = new string[_val];
            StringBuilder builder = new StringBuilder();
            len[0] = _packetData.ReadInt16();
            byte[] data = new byte[len[0]];

            for (int i = 1; i < _val; i++)
            {
                len[i] = _packetData.ReadInt16();
            }
            int u = 0;
            short y = new short();
            bytes[0] = _packetData.Read(data, u, len[1]);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes[0]));
            result[0] = builder.ToString();
            for (short k = 1; k < _val - 1; k++)
            {
                u = u + len[k];
                y = Convert.ToInt16(y + len[k+1]);
                bytes[k] = _packetData.Read(data, u, len[k + 1]);
                builder = new StringBuilder();
                builder.Append(Encoding.Unicode.GetString(data, len[k], bytes[k]));
                result[k] = builder.ToString();
            }
            u = u + len[_val - 1];
            y = Convert.ToInt16(y + len[_val - 1]);
            bytes[_val-1] = _packetData.Read(data, u, len[0]-u);
            builder = new StringBuilder();
            builder.Append(Encoding.Unicode.GetString(data, len[_val - 1], bytes[_val - 1]));
            result[_val-1] = builder.ToString();
            return result;
        }
    }
}
