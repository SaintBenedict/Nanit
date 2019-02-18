﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NaNiT.Functions;

namespace NaNiT.Extensions
{
    public static class BinaryReaderEx
    {
        public static byte[] ReadStarByteArray(this BinaryReader read)
        {
            ulong size = read.ReadVarUInt64();
            return read.ReadBytes((int)size);
        }

        public static string ReadStarString(this BinaryReader read)
        {
            ulong size = read.ReadVarUInt64();
            if (size > 0)
                return Encoding.UTF8.GetString(read.ReadBytes((int)size));
            else
                return "";
        }

        public static byte[] ReadStarUUID(this BinaryReader read)
        {
            bool exists = read.ReadBoolean();
            if(exists)
                return read.ReadBytes(16);
            return new byte[16];
        }

        public static short ReadInt16BE(this BinaryReader read)
        {
            byte[] buffer = read.ReadBytes(2);
            Array.Reverse(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }

        public static int ReadInt32BE(this BinaryReader read)
        {
            byte[] buffer = read.ReadBytes(4);
            Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static uint ReadUInt32BE(this BinaryReader read)
        {
            byte[] buffer = read.ReadBytes(4);
            Array.Reverse(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static long ReadInt64BE(this BinaryReader read)
        {
            byte[] buffer = read.ReadBytes(8);
            Array.Reverse(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static float ReadSingleBE(this BinaryReader read)
        {
            byte[] buffer = read.ReadBytes(4);
            Array.Reverse(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }

        public static double ReadDoubleBE(this BinaryReader read)
        {
            byte[] buffer = read.ReadBytes(8);
            Array.Reverse(buffer);
            return BitConverter.ToDouble(buffer, 0);
        }

        public static byte ReadVarByte(this BinaryReader read)
        {
            var result = ToTarget(read, 8);
            return (byte)result;
        }

        public static short ReadVarInt16(this BinaryReader read)
        {
            var result = ToTarget(read, 16);
            return (short)Decode((short)result);
        }

        public static ushort ReadVarUInt16(this BinaryReader read)
        {
            var result = ToTarget(read, 16);
            return (ushort)result;
        }

        public static int ReadVarInt32(this BinaryReader read)
        {
            var result = ToTarget(read, 32);
            return (int)Decode((int)result);
        }

        public static uint ReadVarUInt32(this BinaryReader read)
        {
            var result = ToTarget(read, 32);
            return (uint)result;
        }

        public static long ReadVarInt64(this BinaryReader read)
        {
            var result = ToTarget(read, 64);
            return Decode((long)result);
        }

        public static ulong ReadVarUInt64(this BinaryReader read)
        {
            var result = ToTarget(read, 64);
            return result;
        }

        private static long Decode(long value)
        {
            if ((value & 1) == 0x00)
                return (value >> 1);
            else
                return -(value >> 1);
        }

        private static ulong ToTarget(BinaryReader read, int sizeBites)
        {
            var buffer = new byte[10];
            var pos = 0;

            int shift = 0;
            ulong result = 0;

            for (;;)
            {
                ulong byteValue = read.ReadByte();
                buffer[pos++] = (byte)byteValue;

                result = (result << 7) | byteValue & 0x7f;

                if (shift > sizeBites)
                    throw new OverflowException("Variable length quantity is too long. (must be " + sizeBites + ")");

                if ((byteValue & 0x80) == 0x00)
                {
                    var bytes = new byte[pos];
                    Buffer.BlockCopy(buffer, 0, bytes, 0, pos);
                    return result;
                }

                shift += 7;
            }

            throw new ArithmeticException("Cannot decode variable length quantity from stream.");
        }

        public static byte[] ReadFully(this BinaryReader read, int requiredSize)
        {
            byte[] buffer = new byte[requiredSize];
            MemoryStream ms = new MemoryStream();
            int curSize = 0;

            while(curSize < requiredSize)
            {
                int size = read.Read(buffer, 0, requiredSize - curSize);
                ms.Write(buffer, 0, size);
                curSize += size;
            }
            return ms.ToArray();
        }
    }
}
