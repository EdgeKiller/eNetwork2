using System;
using System.Collections.Generic;
using System.IO;

namespace eNetwork2
{
    public class PacketWriter
    {
        private List<Byte> buffer;
        public int Position { get; set; }

        public PacketWriter()
        {
            buffer = new List<Byte>();
            Position = 0;
        }

        public void WriteInt16(Int16 value)
        {
            buffer.Add((Byte)value);
            buffer.Add((Byte)(value >> 8));
            Position += 2;
        }

        public void OverWriteInt16(Int16 value)
        {
            if (Position + 1 < buffer.Count)
            {
                buffer[Position] = (Byte)value;
                buffer[Position + 1] = (Byte)(value >> 8);
                Position += 2;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        public void WriteInt32(Int32 value)
        {
            buffer.Add((Byte)value);
            buffer.Add((Byte)(value >> 8));
            buffer.Add((Byte)(value >> 16));
            buffer.Add((Byte)(value >> 24));
            Position += 4;
        }

        public void OverWriteInt32(Int32 value)
        {
            if (Position + 3 < buffer.Count)
            {
                buffer[Position] = (Byte)value;
                buffer[Position + 1] = (Byte)(value >> 8);
                buffer[Position + 2] = (Byte)(value >> 16);
                buffer[Position + 3] = (Byte)(value >> 24);
                Position += 4;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        public void WriteInt64(Int64 value)
        {
            buffer.Add((Byte)value);
            buffer.Add((Byte)(value >> 8));
            buffer.Add((Byte)(value >> 16));
            buffer.Add((Byte)(value >> 24));
            buffer.Add((Byte)(value >> 32));
            buffer.Add((Byte)(value >> 40));
            buffer.Add((Byte)(value >> 48));
            buffer.Add((Byte)(value >> 56));
            Position += 8;
        }

        public void OverWriteInt64(Int64 value)
        {
            if (Position + 7 < buffer.Count)
            {
                buffer[Position] = (Byte)value;
                buffer[Position + 1] = (Byte)(value >> 8);
                buffer[Position + 2] = (Byte)(value >> 16);
                buffer[Position + 3] = (Byte)(value >> 24);
                buffer[Position + 4] = (Byte)(value >> 32);
                buffer[Position + 5] = (Byte)(value >> 40);
                buffer[Position + 6] = (Byte)(value >> 48);
                buffer[Position + 7] = (Byte)(value >> 56);
                Position += 8;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        public void WriteBoolean(Boolean value)
        {
            buffer.Add((Byte)(value == true ? 1 : 0));
            Position++;
        }

        public void OverWriteBoolean(Boolean value)
        {
            if (Position < buffer.Count)
            {
                buffer[Position] = (Byte)(value == true ? 1 : 0);
                Position++;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        public void WriteByte(Byte value)
        {
            buffer.Add(value);
            Position++;
        }

        public void OverWriteByte(Byte value)
        {
            if (Position < buffer.Count)
            {
                buffer[Position] = value;
                Position++;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        public void WriteChar(Char value)
        {
            buffer.Add((Byte)value);
            Position++;
        }

        public void OverWriteChar(Char value)
        {
            if (Position < buffer.Count)
            {
                buffer[Position] = (Byte)value;
                Position++;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        public void WriteDecimal(Decimal value)
        {
            Byte[] tempBuffer = new Byte[16];
            using (MemoryStream ms = new MemoryStream(tempBuffer))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(value);
                }
            }
            for (int i = 0; i < 16; i++)
            {
                buffer.Add(tempBuffer[i]);
            }
            Position += 16;
        }

        public void OverWriteDecimal(Decimal value)
        {
            if (Position + 16 < buffer.Count)
            {
                Byte[] tempBuffer = new Byte[16];
                using (MemoryStream ms = new MemoryStream(tempBuffer))
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(value);
                    }
                }
                for (int i = 0; i < 16; i++)
                {
                    buffer[Position + i] = tempBuffer[i];
                }
                Position += 16;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        public void WriteDouble(Double value)
        {
            Byte[] tempBuffer = new Byte[8];
            using (MemoryStream ms = new MemoryStream(tempBuffer))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(value);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                buffer.Add(tempBuffer[i]);
            }
            Position += 8;
        }

        public void OverWriteDouble(Double value)
        {
            if (Position + 8 < buffer.Count)
            {
                Byte[] tempBuffer = new Byte[8];
                using (MemoryStream ms = new MemoryStream(tempBuffer))
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(value);
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    buffer[Position + i] = tempBuffer[i];
                }
                Position += 8;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        public void WriteString(String value)
        {
            WriteInt16((Int16)value.Length);
            foreach (Char c in value)
            {
                WriteChar(c);
            }
        }

        public void OverWriteString(String value)
        {
            if (Position + value.ToCharArray().Length + 2 <= buffer.Count)
            {
                OverWriteInt16((Int16)value.Length);
                foreach (Char c in value)
                {
                    OverWriteChar(c);
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        public Byte[] ToArray()
        {
            return buffer.ToArray();
        }
    }
}
