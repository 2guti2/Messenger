using System;
using System.Collections.Generic;
using System.IO;

namespace Protocol
{
    public class FileReader
    {
        public const int MaxChunkSize = 32000; // 32kb
        private readonly FileStream stream;

        public FileReader(string path)
        {
            stream = File.OpenRead(path);
        }

        public IEnumerable<byte[]> FileChunks()
        {
            var chunk = new byte[MaxChunkSize];
            while (true)
            {
                int bytesRead = ReadChunk(chunk);

                var refinedChunk = new byte[bytesRead];
                Array.Copy(chunk, refinedChunk, bytesRead);

                if (bytesRead != 0)
                {
                    yield return refinedChunk;
                }
                if (bytesRead != chunk.Length)
                {
                    yield break;
                }
            }
        }

        public void CloseFile()
        {
            stream.Close();
        }

        public int ExpectedChunks()
        {
            double chunks = Convert.ToDouble(stream.Length / (double) MaxChunkSize);
            return Convert.ToInt32(Math.Ceiling(chunks));
        }

        private int ReadChunk(byte[] chunk)
        {
            var index = 0;
            while (index < chunk.Length)
            {
                int bytesRead = stream.Read(chunk, index, chunk.Length - index);
                if (bytesRead == 0)
                {
                    break;
                }
                index += bytesRead;
            }
            return index;
        }
    }
}