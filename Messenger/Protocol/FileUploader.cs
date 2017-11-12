namespace Protocol
{
    public class FileUploader : FileStreamer
    {
        
        public int ExpectedTicks => Reader.ExpectedChunks();
        private FileReader Reader { get; }

        public FileUploader(string filePath)
        {
            Reader = new FileReader(filePath);
        }

        public void UploadFile(Connection conn)
        {
            foreach (byte[] chunk in Reader.FileChunks())
            {
                conn.SendRawData(chunk);
                ProgressMade();
            }
            Reader.CloseFile();
            OperationCompleted();
        }
    }
}