using Autofac;
using MovieAnalyzer.Interfaces.Storage;
using MovieAnalyzer.Storage.AWS;

namespace MovieAnalyzer.Console.Discovery
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

            builder.RegisterType<AWSStorageClientClient>().As<IStorageClient>().InstancePerLifetimeScope().ExternallyOwned();

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
