using Protocol;

namespace ChatServer
{
    public class FileCopier
    {
        public static void CopyFile(string from, string to)
        {
            var reader = new FileReader(from);
            var writer = new FileWriter(to);

            foreach (byte[] chunk in reader.FileChunks())
            {
                writer.WriteChunk(chunk);
            }
            reader.CloseFile();
            writer.CloseFile();
        }
    }
}