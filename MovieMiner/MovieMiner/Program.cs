using MovieMiner.DiscoveryHost;

namespace MovieMiner.Console
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            DiscoverModules discoverModules = new DiscoverModules();
            discoverModules.Start();
        }

    }
}
