using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MovieMiner.Interfaces.Modules;
using MovieMiner.Storage.AWS;

namespace MovieMiner.Console.Modules
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
            string assemblyName =
                Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location);

            if (String.IsNullOrEmpty(assemblyName))
            {
                throw new ApplicationException("Could not reflect over running assembly.");
            }

            catalog.Catalogs.Add(new DirectoryCatalog(assemblyName));

            //Create the CompositionContainer with the parts in the catalog
            CompositionContainer container = new CompositionContainer(catalog);

            //Fill the imports of this object
            container.ComposeParts(this);
        }

        public void Start()
        {
            Parallel.ForEach(_modules, module => module.StartModule<AWSStorageClient>(module.ModuleName));
        }
    }
}
