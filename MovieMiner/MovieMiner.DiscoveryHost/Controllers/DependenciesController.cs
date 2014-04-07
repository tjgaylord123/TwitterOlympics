using Autofac;
using MovieMiner.Interfaces.Storage;
using MovieMiner.Storage.Local;

namespace MovieMiner.DiscoveryHost.Controllers
{
    public class DependenciesController
    {
        private readonly IContainer _container;

        #region Singleton

        private static readonly DependenciesController _instance = new DependenciesController();

        public static DependenciesController Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        private DependenciesController() 
        {
            _container = BuildDependencyGraph();
        }

        private static IContainer BuildDependencyGraph() 
        {
            var builder = new ContainerBuilder();

#if DEBUG
            builder.RegisterType<LocalStorageClient>().As<IStorageClient>().InstancePerLifetimeScope().ExternallyOwned();
#else
            builder.RegisterType<AWSStorageClient>().As<IStorageClient>().InstancePerLifetimeScope().ExternallyOwned();
#endif

            // Discover our modules and register them here
            

            return builder.Build();
        }

        public ILifetimeScope Container  
        {
            get 
            {
                return _container.BeginLifetimeScope();
            }
        }
    }
}
