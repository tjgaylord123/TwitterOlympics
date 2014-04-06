using System;
using System.Threading.Tasks;
using MovieMiner.Interfaces.Storage;

namespace MovieMiner.Interfaces.Modules
{
    public interface IDataModule : IDisposable
    {
        Task StartModule(IStorageClient storageClient);

        string ModuleName { get; }
    }
}
