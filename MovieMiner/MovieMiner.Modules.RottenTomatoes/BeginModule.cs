using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MovieMiner.Interfaces.Modules;
using MovieMiner.Interfaces.Storage;

namespace MovieMiner.Modules.RottenTomatoes
{
    [Export(typeof(IDataModule))]
    public class BeginModule : IDataModule
    {
        private const string ServiceProviderName = "RottenTomatoes";
        public Task StartModule(IStorageClient storageClient)
        {
            var task = (new Task(() => { }));
            task.Start();
            return task;
        }

        public string ModuleName
        {
            get { return ServiceProviderName; }
        }

        public void Dispose()
        {
            
        }
    }
}
