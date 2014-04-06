using System.Threading.Tasks;

namespace MovieMiner.Interfaces.Storage
{
    public interface IStorageClient
    {
        Task<bool> WriteFileToStorageAsync(string filename, byte[] fileContent);
    }
}
