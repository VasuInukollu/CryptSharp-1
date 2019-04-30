﻿namespace CryptSharp.Internal
{
    static class BitPacking
    {
        public static uint UInt32FromBEBytes(byte[] bytes, int offset)
        {
            return
                (uint)bytes[offset + 0] << 24 |
                (uint)bytes[offset + 1] << 16 |
                (uint)bytes[offset + 2] << 8 |
                (uint)bytes[offset + 3];
        }

        public static ulong UInt64FromBEBytes(byte[] bytes, int offset)
        {
            return
                (ulong)bytes[offset + 0] << 56 |
                (ulong)bytes[offset + 1] << 48 |
                (ulong)bytes[offset + 2] << 40 |
                (ulong)bytes[offset + 3] << 32 |
                (ulong)bytes[offset + 4] << 24 |
                (ulong)bytes[offset + 5] << 16 |
                (ulong)bytes[offset + 6] << 8 |
                (ulong)bytes[offset + 7];
        }

        public static uint UInt24FromLEBytes(byte[] bytes, int offset)
        {
            return
                (uint)bytes[offset + 2] << 16 |
                (uint)bytes[offset + 1] << 8 |
                (uint)bytes[offset + 0];
        }

        public static uint UInt32FromLEBytes(byte[] bytes, int offset)
        {
            return
                (uint)bytes[offset + 3] << 24 |
                UInt24FromLEBytes(bytes, offset);
        }

        public static void BEBytesFromUInt32(uint value, byte[] bytes, int offset)
        {
            bytes[offset + 0] = (byte)(value >> 24);
            bytes[offset + 1] = (byte)(value >> 16);
            bytes[offset + 2] = (byte)(value >> 8);
            bytes[offset + 3] = (byte)(value);
        }

        public static void BEBytesFromUInt64(ulong value, byte[] bytes, int offset)
        {
            bytes[offset + 0] = (byte)(value >> 56);
            bytes[offset + 1] = (byte)(value >> 48);
            bytes[offset + 2] = (byte)(value >> 40);
            bytes[offset + 3] = (byte)(value >> 32);
            bytes[offset + 4] = (byte)(value >> 24);
            bytes[offset + 5] = (byte)(value >> 16);
            bytes[offset + 6] = (byte)(value >> 8);
            bytes[offset + 7] = (byte)(value);
        }

        public static void LEBytesFromUInt24(uint value, byte[] bytes, int offset)
        {
            bytes[offset + 2] = (byte)(value >> 16);
            bytes[offset + 1] = (byte)(value >> 8);
            bytes[offset + 0] = (byte)(value);
        }

        public static void LEBytesFromUInt32(uint value, byte[] bytes, int offset)
        {
            bytes[offset + 3] = (byte)(value >> 24);
            LEBytesFromUInt24(value, bytes, offset);
        }
    }
}