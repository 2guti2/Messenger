using System.IO;

namespace Business
{
    public class FileWriter
    {
        private readonly FileStream stream;
        
        public FileWriter(string path)
        {
            stream = File.OpenWrite(path);
        }

        public void WriteChunk(byte[] chunk)
        {
            stream.Write(chunk, 0, chunk.Length);
        }

        public void CloseFile()
        {
            stream.Flush();
            stream.Close();
        }
    }
}