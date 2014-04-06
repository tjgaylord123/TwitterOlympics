using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieMiner.Interfaces.Storage;

namespace MovieMiner.Storage.AWS
{
    public class AWSStorageClient : IStorageClient
    {
        public Task<bool> WriteFileToStorageAsync(string fileContent, DateTime folderDate, FileType fileType)
        {
            throw new NotImplementedException();
        }

        public DateTime GetLatestDateFolderInStorage()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }
    }
}
