using System;
using MovieAnalyzer.Interfaces.Storage;

namespace MovieAnalyzer.Storage.AWS
{
    public class AWSStorageClient : IStorage
    {

        public AWSStorageClient(string directory)
        {
            
        }

        public string GetFileContents(string fileName)
        {
            return String.Empty;
        }
    }
}
