using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using MovieMiner.Interfaces.Storage;

namespace MovieMiner.Storage.AWS
{
    public class AWSStorageClient : IStorageClient
    {
        private const string BucketName = "blackjack-movieminer";
        private static readonly DateTime _minDate = new DateTime(1930, 1, 1);

        private readonly S3DirectoryInfo _rootDirectory;
        private readonly S3DirectoryInfo _subDirectory;
        private readonly string _apiClientName;
        private IAmazonS3 _storageClient;
        private DateTime _latestDate;

        public AWSStorageClient(string specificFolder)
        {
            _apiClientName = specificFolder;
            _storageClient = AWSClientFactory.CreateAmazonS3Client();
            _rootDirectory = new S3DirectoryInfo(_storageClient, BucketName);
            _rootDirectory.Create();

            _subDirectory = _rootDirectory.CreateSubdirectory(_apiClientName);

            // Get the correct subdirectory
            var fileNames = _subDirectory.EnumerateFiles().Select(file => file.Name).ToArray();
            _latestDate = fileNames.Length > 0
                ? fileNames
                    .Max(
                        fileName =>
                        {
                            int index = fileName.IndexOf('.');
                            var date = DateTime.Parse(fileName.Substring(0, index));
                            return date;
                        }).AddDays(1)
                : _minDate;
        }

        public async Task<bool> WriteFileToStorageAsync(string fileContent, DateTime dateFileName, FileType fileType)
        {
            try
            {
                S3FileInfo newFile = _subDirectory.GetFile(string.Format("{0}.{1}", dateFileName.ToString("yyyy MMMM dd"), GetFileTypeExtension(fileType)));
                // Write the file
                using (StreamWriter writer = new StreamWriter(newFile.OpenWrite()))
                {
                    await writer.WriteAsync(fileContent);
                }
            }
            catch {
                return false;
            }

            _latestDate = dateFileName.AddDays(1);
            return true;
        }

        public async Task<bool> WriteFileToStorageAsync(string fileContent, string genericFileName, FileType fileType)
        {
            try
            {
                S3FileInfo newFile = _subDirectory.GetFile(string.Format("{0}.{1}", genericFileName, GetFileTypeExtension(fileType))); 
                // Write the file
                using (StreamWriter writer = new StreamWriter(newFile.OpenWrite()))
                {
                    await writer.WriteAsync(fileContent);
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
            return _latestDate;
        }

        private static string GetFileTypeExtension(FileType fileType)
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

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _storageClient == null) return;

            _storageClient.Dispose();
            _storageClient = null;
        }

        #endregion

    }
}
