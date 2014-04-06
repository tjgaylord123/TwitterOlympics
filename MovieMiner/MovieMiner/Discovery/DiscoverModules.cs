using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using MovieMiner.Console.Annotations;
using MovieMiner.Interfaces.Modules;

namespace MovieMiner.Console.Discovery
{
    internal class DiscoverModules
    {
        [ImportMany(typeof(IDataModule)), UsedImplicitly]
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

        public IEnumerable<IDataModule> Modules
        {
            get { return _modules; }
        }
    }
}
