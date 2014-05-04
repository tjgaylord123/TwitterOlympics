using System;
using System.Threading.Tasks;
using MovieAnalyzer.Interfaces.Storage;

namespace MovieAnalyzer.Interfaces.Modules
{
    public interface IJobModule : IDisposable
    {

        Task BeginJobModule(IStorageClient storageClient);

        string ModuleName { get; }

    }
}
