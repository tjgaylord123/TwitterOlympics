using System;
using System.Threading.Tasks;

namespace MovieMiner.Interfaces.Storage
{
    public interface IStorageClient : IDisposable
    {
        Task<bool> WriteFileToStorageAsync(string fileContent, DateTime dateFileName, FileType fileType);

        Task<bool> WriteFileToStorageAsync(string fileContent, string genericFileName, FileType fileType);

        DateTime GetLatestDateFolderInStorage();
    }

}
