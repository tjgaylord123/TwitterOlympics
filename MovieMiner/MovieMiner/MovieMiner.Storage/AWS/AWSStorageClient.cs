using System;
using System.IO;
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

        private readonly S3DirectoryInfo _subDirectory;
        private IAmazonS3 _storageClient;
        private DateTime _latestDate;

        public AWSStorageClient(string specificFolder)
        {
            _storageClient = AWSClientFactory.CreateAmazonS3Client();
            S3DirectoryInfo rootDirectory = new S3DirectoryInfo(_storageClient, BucketName);
            rootDirectory.Create();

            _subDirectory = rootDirectory.CreateSubdirectory(specificFolder);

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
