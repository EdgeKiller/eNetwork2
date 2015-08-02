using System;
using System.IO;
using System.Text;

namespace eNetwork2
{
    public class PacketReader
    {
        private byte[] buffer;
        public int Position { get; set; }

        public PacketReader(byte[] buffer)
        {
            this.buffer = buffer;
            Position = 0;
        }

        public Int16 ReadInt16()
        {
            Int16 result = (Int16)(buffer[Position] | buffer[Position + 1] << 8);
            Position += 2;
            return result;
        }

        public Int32 ReadInt32()
        {
            Int32 result = (Int32)(buffer[Position] | buffer[Position + 1] << 8 | buffer[Position + 2] << 16 | buffer[Position + 3] << 24);
            Position += 4;
            return result;
        }

        public Int64 ReadInt64()
        {
            Int64 result = (Int64)(buffer[Position] | buffer[Position + 1] << 8 | buffer[Position + 2] << 16 | buffer[Position + 3] << 24 |
                buffer[Position + 4] << 32 | buffer[Position + 5] << 40 | buffer[Position + 6] << 48 | buffer[Position + 7] << 56);
            Position += 8;
            return result;
        }

        public Boolean ReadBoolean()
        {
            Boolean result = (buffer[Position] == 1 ? true : false);
            Position++;
            return result;
        }

        public Byte ReadByte()
        {
            Byte result = (buffer[Position]);
            Position++;
            return result;
        }

        public Char ReadChar()
        {
            Char result = (Char)buffer[Position];
            Position++;
            return result;
        }

        public Decimal ReadDecimal()
        {
            Decimal result;
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    br.BaseStream.Position = Position;
                    result = br.ReadDecimal();
                }
            }
            Position += 16;
            return result;
        }

        public Double ReadDouble()
        {
            Double result;
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    br.BaseStream.Position = Position;
                    result = br.ReadDouble();
                }
            }
            Position += 8;
            return result;
        }

        public String ReadString()
        {
            StringBuilder sb = new StringBuilder();
            Int16 size = ReadInt16();
            for (int i = 0; i < size; i++)
            {
                sb.Append(ReadChar());
            }
            return sb.ToString();
        }
    }
}
