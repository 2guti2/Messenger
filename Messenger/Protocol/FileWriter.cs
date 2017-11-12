using System.IO;

namespace Protocol
{
    public class FileWriter
    {
        private readonly FileStream stream;
        
        public FileWriter(string path)
        {
            string fileDirectory = Path.GetDirectoryName(path);
            if (!Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);
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