using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Model
{
    public class ByteArrayFormatter
    {
        public static byte[] Format(string message)
        {
            return Encoding.ASCII.GetBytes(message);
        }

        public static string DeserializeMessage(byte[] data, int count)
        {
            return Encoding.ASCII.GetString(data, 0, count);
        }
    }
}
