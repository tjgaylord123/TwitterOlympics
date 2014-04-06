using System;
using System.Threading.Tasks;

namespace MovieMiner.Interfaces.Storage
{
    public interface IStorageClient : IDisposable
    {
        Task<bool> WriteFileToStorageAsync(string fileContent, DateTime folderDate, FileType fileType);

        DateTime GetLatestDateFolderInStorage();
    }

}
