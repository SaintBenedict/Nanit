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

        public static void WriteToString(Packet _id, BinaryWriter _bw, string _message)
        {
            MemoryStream packet = new MemoryStream();
            BinaryWriter packetWrite = new BinaryWriter(packet);
            byte[] buffer = Encoding.Unicode.GetBytes(_message);
            packetWrite.Write((short)buffer.Length);
            packetWrite.Write(buffer);
            byte[] message = packet.ToArray();
            _bw.Write((short)_id);
            _bw.Write(message.Length);
            _bw.Write(message);
            _bw.Flush();
        }

        public static void WriteToString(Packet _id, BinaryWriter _bw, params string[] _message)
        {
            MemoryStream packet = new MemoryStream();
            BinaryWriter packetWrite = new BinaryWriter(packet);
            byte[] message;
            string allstring = "";

            for (int i = 0; i < _message.Length; i++)
            {
                allstring = allstring + _message[i];
            }

            byte[] buffer;
            byte[] bufferAll = Encoding.Unicode.GetBytes(allstring);
            packetWrite.Write((short)bufferAll.Length);

            for (int k = 0; k < _message.Length; k++)
            {
                buffer = Encoding.Unicode.GetBytes(_message[k]);
                packetWrite.Write((short)buffer.Length);
            }
            packetWrite.Write(bufferAll);
            message = packet.ToArray();
            _bw.Write((short)_id);
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
            short[] len = new short[_val+1];
            string[] result = new string[_val];
            StringBuilder builder = new StringBuilder();
            

            for (int i = 0; i < _val+1; i++)
            {
                len[i] = _packetData.ReadInt16();
            }
            byte[] data = new byte[len[0] - (_val+1)*2];
            int f = 0;
            for (short k = 1; k < _val+1; k++)
            {
                builder = new StringBuilder();
                int bytes = 0;
                bytes = _packetData.Read(data, 0, Convert.ToInt16(len[k]));
                builder.Append(Encoding.Unicode.GetString(data, 0, len[k]));
                result[k-1] = builder.ToString();
            }
            return result;
        }
    }
}
