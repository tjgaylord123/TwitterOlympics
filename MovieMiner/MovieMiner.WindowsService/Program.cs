using System.ServiceProcess;

namespace MovieMiner.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] servicesToRun =
            { 
                new ScheduledService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
