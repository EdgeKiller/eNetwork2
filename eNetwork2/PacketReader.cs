using System;
using System.IO;

namespace eNetwork2
{
    public static class PacketReader
    {
        private static MemoryStream ms;
        private static BinaryReader br;

        public static Int16 ReadInt16(byte[] buffer)
        {
            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);
            return br.ReadInt16();
        }

        public static Int32 ReadInt32(byte[] buffer)
        {
            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);
            return br.ReadInt32();
        }

        public static Int64 ReadInt64(byte[] buffer)
        {
            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);
            return br.ReadInt64();
        }

        public static Boolean ReadBoolean(byte[] buffer)
        {
            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);
            return br.ReadBoolean();
        }

        public static Byte ReadByte(byte[] buffer)
        {
            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);
            return br.ReadByte();
        }

        public static Char ReadChar(byte[] buffer)
        {
            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);
            return br.ReadChar();
        }

        public static Decimal ReadDecimal(byte[] buffer)
        {
            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);
            return br.ReadDecimal();
        }

        public static Double ReadDouble(byte[] buffer)
        {
            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);
            return br.ReadDouble();
        }

        public static String ReadString(byte[] buffer)
        {
            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);
            return br.ReadString();
        }
    }
}
