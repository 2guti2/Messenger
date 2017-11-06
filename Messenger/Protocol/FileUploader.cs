using Business;

namespace Protocol
{
    public class FileUploader
    {
        public static void UploadFile(Connection conn, string filePath)
        {
            var reader = new FileReader(filePath);
            foreach (byte[] chunk in reader.FileChunks())
                conn.SendRawData(chunk);
        }
    }
}