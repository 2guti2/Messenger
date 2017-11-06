using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Business
{
    public static class FileLister
    {
        public static List<string> ListFiles(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            var directoryInfo = new DirectoryInfo(path);
            FileInfo[] files = directoryInfo.GetFiles();
            
            return new List<string>(files.Select(file => file.Name));
        }
    }
}