using System;
using System.IO;
using Business;
using Protocol;

namespace Server
{
    public class FileUploader
    {
        private const string UploadFolder = "uploads";
        
        public static string UploadFile(Connection conn, Client client, string fileName)
        {
            string uploadDirectory = $@"{UploadFolder}\{client.Id}";
            if (!Directory.Exists(uploadDirectory))
                Directory.CreateDirectory(uploadDirectory);

            string uniqueFileName = $"{Timestamp()}_{fileName}";
            var writer = new FileWriter($@"{uploadDirectory}\{uniqueFileName}");
            while(true)
            {
                byte[] chunk = conn.ReadRawData();
                writer.WriteChunk(chunk);
                if (chunk.Length < FileReader.MaxChunkSize) break;
            }
            return uniqueFileName;
        }
        
        private static string Timestamp() {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }
    }
}