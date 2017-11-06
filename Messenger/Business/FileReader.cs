using System;
using System.Collections.Generic;
using System.IO;

namespace Business
{
    public class FileReader
    {
        public const int MaxChunkSize = 32000; // 32kb
        private readonly FileStream stream;

        public FileReader(string path)
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public IEnumerable<byte[]> FileChunks()
        {
            var chunk = new byte[MaxChunkSize];
            while (true)
            {
                int bytesRead = ReadChunk(stream, chunk);
                
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
        
        private int ReadChunk(Stream stream, byte[] chunk)
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