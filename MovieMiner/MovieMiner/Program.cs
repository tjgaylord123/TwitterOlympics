using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using MovieMiner.Console.Controllers;
using MovieMiner.Console.Discovery;
using MovieMiner.Interfaces.Modules;
using MovieMiner.Interfaces.Storage;

namespace MovieMiner.Console
{
    class Program
    {

        static void Main(string[] args)
        {
            DiscoverModules discoverModules = new DiscoverModules();

            
            foreach (var module in discoverModules.Modules)
            {
                IDataModule closureSafeModule = module;
                var task = Task.Run(async () =>
                {
                    var parameters = new Parameter[] { new NamedParameter("specificFolder", closureSafeModule.ModuleName) };
                    await closureSafeModule.StartModule(
                            DependenciesController.Instance.Container.Resolve<IStorageClient>(parameters));
                    closureSafeModule.Dispose();
                });
                task.Wait();
            }
        }

    }
}
