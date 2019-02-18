﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NaNiT.Functions;

namespace NaNiT.Extensions
{
    public static class BinaryWriterEx
    {
        public static void WriteStarString(this BinaryWriter write, string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            write.WriteVarUInt32((uint)buffer.Length);
            write.Write(buffer);
        }
        
        public static void WriteVarByte(this BinaryWriter write, byte value)
        {
            WriteVarUInt64(write, value);
        }

        public static void WriteVarInt16(this BinaryWriter write, short value)
        {
            var encode = Encode(value);
            WriteVarUInt64(write, encode);
        }

        public static void WriteVarUInt16(this BinaryWriter write, ushort value)
        {
            WriteVarUInt64(write, value);
        }

        public static void WriteVarInt32(this BinaryWriter write, int value)
        {
            var encode = Encode(value);
            WriteVarUInt64(write, encode);
        }

        public static void WriteVarUInt32(this BinaryWriter write, uint value)
        {
            WriteVarUInt64(write, value);
        }

        public static void WriteVarInt64(this BinaryWriter write, long value)
        {
            var encode = Encode(value);
            WriteVarUInt64(write, encode);
        }

        private static ulong Encode(long value)
        {
            if (value < 0)
                return (ulong)(-2 * value + 1);
            else
                return (ulong)(2 * value);
        }

        public static void WriteVarUInt64(this BinaryWriter write, ulong value)
        {
            var buffer = new byte[10];
            var pos = 0;
            do
            {
                var byteVal = value & 0x7f;
                value >>= 7;

                if (value != 0)
                {
                    byteVal |= 0x80;
                }

                buffer[pos++] = (byte)byteVal;

            } while (value != 0);

            var result = new byte[pos];
            Buffer.BlockCopy(buffer, 0, result, 0, pos);
            Array.Reverse(result);
            if(pos > 1)
            {
                result[0] |= 0x80;
                result[pos-1] ^= 0x80;
            }

            write.Write(result);
        }

        public static void WriteBE(this BinaryWriter write, ulong value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            write.Write(buffer);
        }

        public static void WriteBE(this BinaryWriter write, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            write.Write(buffer);
        }

        public static void WriteBE(this BinaryWriter write, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            write.Write(buffer);
        }

        public static void WriteBE(this BinaryWriter write, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            write.Write(buffer);
        }

        public static void WriteBE(this BinaryWriter write, ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            write.Write(buffer);
        }

        public static void WriteBE(this BinaryWriter write, short value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            write.Write(buffer);
        }

        public static void WriteBE(this BinaryWriter write, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            write.Write(buffer);
        }

        public static void WriteBE(this BinaryWriter write, double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            write.Write(buffer);
        }
    }
}
