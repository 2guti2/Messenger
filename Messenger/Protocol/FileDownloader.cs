using System;
using System.IO;
using Business;

namespace Protocol
{
    public class FileDownloader
    {
        public static string DownloadFile(Connection conn, string downloadDirectory, string fileName)
        {
            string uniqueFileName = SetupFileLocation(downloadDirectory, fileName);
            FileWriter writer = BuildFileWriter(downloadDirectory, fileName);
            SaveFile(conn, writer);

            return uniqueFileName;
        }

        private static string SetupFileLocation(string downloadDirectory, string fileName)
        {
            if (!Directory.Exists(downloadDirectory))
                Directory.CreateDirectory(downloadDirectory);

            return $"{Timestamp()}_{fileName}";
        }

        private static FileWriter BuildFileWriter(string downloadDirectory, string fileName)
        {
            return new FileWriter($@"{downloadDirectory}\{fileName}");
        }

        private static void SaveFile(Connection conn, FileWriter writer)
        {
            while (true)
            {
                byte[] chunk = conn.ReadRawData();
                writer.WriteChunk(chunk);
                if (chunk.Length < FileReader.MaxChunkSize) break;
            }
            writer.CloseFile();
        }

        private static string Timestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }
    }
}