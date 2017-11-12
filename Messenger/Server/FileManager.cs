using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Protocol;
using UI;

namespace Server
{
    public class FileManager
    {
        private const string FilesFolder = "files";
        private const string UplaodsFolder = "uploads";
        private const string DownloadsFolder = "downloads";
        private const string UploadSuccessMessage = "File uploaded successfully";
        private const string DownloadSuccessMessage = "File downloaded successfully";

        public void UploadFile()
        {
            List<string> files = FileLister.ListFiles(FilesFolder);
            if (files.Count == 0)
            {
                Console.WriteLine("You have no files to choose from");
                return;
            }
            string selectedFile = SelectFileFromList(files);
            string fileWithTimestamp = FileNameWithTimestamp(selectedFile);
            CopyFile(FullFilePath(FilesFolder, selectedFile), UplaodsFolder, fileWithTimestamp, UploadSuccessMessage);
        }

        public void DownloadFile()
        {
            List<string> files = FileLister.ListFiles(UplaodsFolder);
            List<string> formattedFiles = RemoveTimestampFromFileNames(files);
            if (files.Count == 0)
            {
                Console.WriteLine("There are no files uploaded");
                return;
            }
            int selectedOption = Menus.MapInputWithMenuItemsList(formattedFiles);
            string selectedFile = files[selectedOption - 1];
            string formattedSelectedFile = formattedFiles[selectedOption - 1];
            string destinationFile = FileNameWithTimestamp(formattedSelectedFile);
            CopyFile(FullFilePath(UplaodsFolder, selectedFile), DownloadsFolder, destinationFile,
                DownloadSuccessMessage);
        }

        private string SelectFileFromList(List<string> files)
        {
            int selectedOption = Menus.MapInputWithMenuItemsList(files);
            return files[selectedOption - 1];
        }

        private List<string> RemoveTimestampFromFileNames(IEnumerable<string> files)
        {
            var regex = new Regex(@"^\d+_");
            return files.Select(file => regex.Replace(file, "")).ToList();
        }

        private void CopyFile(string sourcePath, string destinationFolder, string destinationFile,
            string successMessage)
        {
            try
            {
                FileCopier.CopyFile(sourcePath, $@"{destinationFolder}\{destinationFile}");
                Console.WriteLine(successMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine("There was a problem uploading your file");
            }
        }

        private string FullFilePath(string folder, string fileName)
        {
            return $@"{folder}\{fileName}";
        }

        private string FileNameWithTimestamp(string fileName)
        {
            return $"{Timestamp()}_{fileName}";
        }

        private string Timestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }
    }
}