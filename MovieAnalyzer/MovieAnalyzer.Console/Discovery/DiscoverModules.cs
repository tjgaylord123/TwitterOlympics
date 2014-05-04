using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using MovieAnalyzer.Console.Discovery;
using MovieAnalyzer.Interfaces.Modules;
using MovieAnalyzer.Interfaces.Storage;

namespace MovieAnalyzer.Console
{
    internal class DiscoverModules
    {
        [ImportMany(typeof(IJobModule))]
        private IEnumerable<IJobModule> _modules;

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
            // Take advantage of the Task Parallel Library to spread work out accross logical cores
            Parallel.ForEach(_modules, module =>
            {
                IJobModule closureSafeModule = module;
                var task = Task.Run(async () =>
                {
                    var parameters = new Parameter[] { new NamedParameter("directory", closureSafeModule.ModuleName) };
                    await closureSafeModule.BeginJobModule(DependenciesController.Instance.Container.Resolve<IStorage>(parameters));
                    closureSafeModule.Dispose();
                });
                task.Wait();
            });
        }
    }
}
