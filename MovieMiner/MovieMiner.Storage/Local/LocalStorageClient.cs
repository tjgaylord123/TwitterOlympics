using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieMiner.Interfaces.Storage;

namespace MovieMiner.Storage.Local
{
    public class LocalStorageClient : IStorageClient
    {
        private const string RootFolder = @"C:\MovieData\Json\";
        private static readonly DateTime _minimumDate = new DateTime(2014, 1, 1);
        private readonly string _absoluteFolder;

        private readonly Dictionary<DateTime, int> _folderNamesCache; 

        public LocalStorageClient(string specificFolder)
        {
            _folderNamesCache = new Dictionary<DateTime, int>();
            _absoluteFolder = string.Format("{0}{1}", RootFolder, specificFolder);
            if (!Directory.Exists(_absoluteFolder))
            {
                Directory.CreateDirectory(_absoluteFolder);
            }

            foreach (var folder in Directory.EnumerateDirectories(_absoluteFolder))
            {
                var files = Directory.EnumerateFiles(folder).ToArray();
                if (files.Length == 0)
                {
                    _folderNamesCache.Add(DateTime.Parse(folder), 1);
                }
                string folderIdentifier =
                    new string(folder.Reverse().TakeWhile(c => c != Path.DirectorySeparatorChar).Reverse().ToArray());

                _folderNamesCache.Add(
                    DateTime.Parse(folderIdentifier).Date,
                    files.Max(f => int.Parse(Path.GetFileNameWithoutExtension(f))));
            }
        }

        public async Task<bool> WriteFileToStorageAsync(string fileContent, DateTime folderDate, FileType fileType)
        {
            try
            {
                // First create a folder if necessary
                string directoryPath = string.Format("{0}\\{1}", _absoluteFolder, folderDate.ToString("yyyy MMMM dd"));
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Get the next filename and increment
                if (!_folderNamesCache.ContainsKey(folderDate.Date))
                {
                    _folderNamesCache.Add(folderDate.Date, 1);
                }

                string fileName = string.Format("{0}.{1}", _folderNamesCache[folderDate.Date], GetFileTypeExtension(fileType));
                _folderNamesCache[folderDate.Date]++;

                // Write the file
                byte[] encodedText = Encoding.Unicode.GetBytes(fileContent);

                using (FileStream stream = new FileStream(string.Format(@"{0}\{1}", directoryPath, fileName), FileMode.CreateNew, FileAccess.Write))
                {
                    await stream.WriteAsync(encodedText, 0, encodedText.Length);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public DateTime GetLatestDateFolderInStorage()
        {
            string[] movieDataDirectories = Directory.EnumerateDirectories(_absoluteFolder).ToArray();
            return movieDataDirectories.Length == 0
                ? _minimumDate
                : movieDataDirectories.Select(
                    d => DateTime.Parse(new string(
                                d.Reverse().TakeWhile(c => c != Path.DirectorySeparatorChar).Reverse().ToArray())))
                    .Max();
        }

        private string GetFileTypeExtension(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Json:
                    return "json";
                case FileType.Text:
                    return "txt";
                default:
                    return "txt";
            }
        }

        public void Dispose()
        {
            // Do nothing for local storage
        }
    }
}
