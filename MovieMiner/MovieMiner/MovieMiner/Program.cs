using System;
using System.Diagnostics;
using MovieMiner.DiscoveryHost;

namespace MovieMiner.Console
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            System.Console.WriteLine("Starting stopwatch...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            DiscoverModules discoverModules = new DiscoverModules();
            discoverModules.Start();
            stopwatch.Stop();

            System.Console.WriteLine("Elapsed minutes: {0}.{1}", stopwatch.Elapsed.TotalMinutes, Environment.NewLine);
            System.Console.WriteLine("Press enter to quit...");
            System.Console.ReadLine();
        }

    }
}
