using System;
using System.IO;

namespace eNetwork2
{
    class Utils
    {
        public static byte[] GetBuffer(byte[] buffer)
        {
            byte[] result = new byte[buffer.Length + 2];
            using (MemoryStream ms = new MemoryStream(result))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write((Int16)buffer.Length);
                }
            }
            Array.Copy(buffer, 0, result, 2, buffer.Length);
            return result;
        }
    }
}
