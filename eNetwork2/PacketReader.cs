using System;
using System.IO;

namespace eNetwork2
{
    public class PacketReader
    {
        private MemoryStream ms;
        private BinaryReader br;
        private byte[] buffer;

        public PacketReader(byte[] buffer)
        {
            this.buffer = buffer;
            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);
        }

        public Int16 ReadInt16()
        {
            return br.ReadInt16();
        }

        public Int32 ReadInt32()
        {
            return br.ReadInt32();
        }

        public Int64 ReadInt64()
        {
            return br.ReadInt64();
        }

        public Boolean ReadBoolean()
        {
            return br.ReadBoolean();
        }

        public Byte ReadByte()
        {
            return br.ReadByte();
        }

        public Char ReadChar()
        {
            return br.ReadChar();
        }

        public Decimal ReadDecimal()
        {
            return br.ReadDecimal();
        }

        public Double ReadDouble()
        {
            return br.ReadDouble();
        }

        public String ReadString()
        {
            return br.ReadString();
        }
    }
}
