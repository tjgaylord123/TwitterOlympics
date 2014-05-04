using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using MovieMiner.DiscoveryHost.Controllers;
using MovieMiner.Interfaces.Modules;
using MovieMiner.Interfaces.Storage;

namespace MovieMiner.DiscoveryHost
{
    public class DiscoverModules
    {
        [ImportMany(typeof(IDataModule))]
        private IEnumerable<IDataModule> _modules;

        public DiscoverModules()
        {
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in all assemblies in 
            //the same directory as the executing program
            catalog.Catalogs.Add(
             new DirectoryCatalog(
              Path.GetDirectoryName(
               Assembly.GetExecutingAssembly().Location)));

            //Create the CompositionContainer with the parts in the catalog
            CompositionContainer container = new CompositionContainer(catalog);

            //Fill the imports of this object
            container.ComposeParts(this);
        }

        public void Start()
        {
            Parallel.ForEach(_modules, module =>
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
            });
        }
    }
}
