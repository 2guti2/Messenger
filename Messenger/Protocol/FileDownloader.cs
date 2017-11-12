using System;
using System.IO;

namespace Protocol
{
    public class FileDownloader : FileStreamer
    {

        public string FilePath { get; }

        public FileDownloader(string downloadDirectory, string fileName)
        {
            FilePath = SetupFileLocation(downloadDirectory, fileName);
        }

        public void DownloadFile(Connection conn)
        {
            FileWriter writer = BuildFileWriter();
            SaveFile(conn, writer);
        }

        private string SetupFileLocation(string downloadDirectory, string fileName)
        {
            if (!Directory.Exists(downloadDirectory))
                Directory.CreateDirectory(downloadDirectory);

            return $@"{downloadDirectory}\{Timestamp()}_{fileName}";
        }

        private FileWriter BuildFileWriter()
        {
            return new FileWriter(FilePath);
        }

        private void SaveFile(Connection conn, FileWriter writer)
        {
            while (true)
            {
                byte[] chunk = conn.ReadRawData();
                writer.WriteChunk(chunk);
                ProgressMade();
                if (chunk.Length < FileReader.MaxChunkSize) break;
            }
            writer.CloseFile();
            OperationCompleted();
        }

        private string Timestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }
    }
}