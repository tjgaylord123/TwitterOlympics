using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using MovieAnalyzer.Interfaces.Storage;
using MovieMiner.Interfaces.Storage;

namespace MovieAnalyzer.Storage.AWS
{
    public class AWSStorageClientClient : IStorageClient
    {
        private const string BucketName = "blackjack-movieminer";

        private readonly S3DirectoryInfo _mainDirectory, _outputDirectory;
        private IAmazonS3 _storageClient;

        public AWSStorageClientClient(string directory)
        {
            _storageClient = AWSClientFactory.CreateAmazonS3Client();
            S3DirectoryInfo rootDirectory = new S3DirectoryInfo(_storageClient, BucketName);
            rootDirectory.Create();

            _mainDirectory = rootDirectory.CreateSubdirectory(directory);
            _outputDirectory = _mainDirectory.CreateSubdirectory("Output");
        }

        public string[] GetAllFileNames()
        {
            return _mainDirectory.EnumerateFiles().Select(file =>
            {
                int index = file.Name.IndexOf('.');
                return file.Name.Substring(0, index);
            }).ToArray();
        }

        public async Task<string> GetFileContents(string fileName, FileType fileType)
        {
            // Get the fully qualified file name
            string fullFileName = string.Format("{0}.{1}", fileName, GetFileTypeExtension(fileType));

            S3FileInfo serverFile = _mainDirectory.GetFile(fullFileName);

            try
            {
                using (StreamReader reader = new StreamReader(serverFile.OpenRead()))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        public async Task<bool> WriteFileToStorageAsync(string fileContent, string fileName, FileType fileType, bool isOutputDirectory = false)
        {
            try
            {
                S3FileInfo newFile = isOutputDirectory
                    ? _outputDirectory.GetFile(string.Format("{0}.{1}", fileName, GetFileTypeExtension(fileType)))
                    : _mainDirectory.GetFile(string.Format("{0}.{1}", fileName, GetFileTypeExtension(fileType)));

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
