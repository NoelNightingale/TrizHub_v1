#region Usings

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

#endregion

namespace TCR.Lib.Utility
{
    public class Compression
    {
        public static byte[] Compress(byte[] input)
        {
            if (input == null)
                return null;
            using (var output = new MemoryStream())
            {
                using (var zip = new GZipStream(output, CompressionMode.Compress))
                {
                    zip.Write(input, 0, input.Length);
                }
                return output.ToArray();
            }
        }

        public static byte[] Decompress(byte[] input)
        {
            if (input == null)
                return null;
            using (var output = new MemoryStream(input))
            {
                using (var zip = new GZipStream(output, CompressionMode.Decompress))
                {
                    var bytes = new List<byte>();
                    var b = zip.ReadByte();
                    while (b != -1)
                    {
                        bytes.Add((byte) b);
                        b = zip.ReadByte();
                    }
                    return bytes.ToArray();
                }
            }
        }

        private static byte[] ReadAllBytesLocal(string fileName)
        {
            byte[] buffer;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int) fs.Length);
            }
            return buffer;
        }
    }
}