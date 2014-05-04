using MovieMiner.Interfaces.Storage;

namespace MovieMiner.Interfaces.Modules
{
    public interface IDataModule
    {
        void StartModule<TStorage>(string directory) where TStorage : IStorageClient;

        string ModuleName { get; }
    }
}
