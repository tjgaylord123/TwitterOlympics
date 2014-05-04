using System;
using System.Threading.Tasks;
using MovieMiner.Interfaces.Storage;

namespace MovieAnalyzer.Interfaces.Storage
{
    public interface IStorageClient : IDisposable
    {
        string[] GetAllFileNames();

        Task<string> GetFileContents(string fileName, FileType fileType);

        Task<bool> WriteFileToStorageAsync(string fileContent, string fileName, FileType fileType, bool isOutputDirectory = false);
    }
}
