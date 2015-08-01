using System;
using System.IO;

namespace eNetwork2
{
    public static class PacketWriter
    {
        private static MemoryStream ms;
        private static BinaryWriter bw;

        public static void WriteInt16(ref byte[] buffer, Int16 value)
        {
            ms = new MemoryStream(buffer);
            bw = new BinaryWriter(ms);
            bw.Write(value);
        }

        public static void WriteInt32(ref byte[] buffer, Int32 value)
        {
            ms = new MemoryStream(buffer);
            bw = new BinaryWriter(ms);
            bw.Write(value);
        }

        public static void WriteInt64(ref byte[] buffer, Int64 value)
        {
            ms = new MemoryStream(buffer);
            bw = new BinaryWriter(ms);
            bw.Write(value);
        }

        public static void WriteBoolean(ref byte[] buffer, Boolean value)
        {
            ms = new MemoryStream(buffer);
            bw = new BinaryWriter(ms);
            bw.Write(value);
        }

        public static void WriteByte(ref byte[] buffer, Byte value)
        {
            ms = new MemoryStream(buffer);
            bw = new BinaryWriter(ms);
            bw.Write(value);
        }

        public static void WriteChar(ref byte[] buffer, Char value)
        {
            ms = new MemoryStream(buffer);
            bw = new BinaryWriter(ms);
            bw.Write(value);
        }

        public static void WriteDecimal(ref byte[] buffer, Decimal value)
        {
            ms = new MemoryStream(buffer);
            bw = new BinaryWriter(ms);
            bw.Write(value);
        }

        public static void WriteDouble(ref byte[] buffer, Double value)
        {
            ms = new MemoryStream(buffer);
            bw = new BinaryWriter(ms);
            bw.Write(value);
        }
        
        public static void WriteString(ref byte[] buffer, String value)
        {
            ms = new MemoryStream(buffer);
            bw = new BinaryWriter(ms);
            bw.Write(value);
        }
    }
}
