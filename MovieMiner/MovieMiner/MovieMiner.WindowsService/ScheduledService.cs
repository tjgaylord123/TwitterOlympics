using System.ServiceProcess;
using System.Threading;

namespace MovieMiner.WindowsService
{
    public partial class ScheduledService : ServiceBase
    {
        private object lockObject;
        private bool stopped;
        public ScheduledService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            stopped = false;
            while (!stopped)
            {
                DiscoverModules discoverModules = new DiscoverModules();
                discoverModules.Start();
                Thread.Sleep(100000);
            }
        }

        protected override void OnStop()
        {
            lock (lockObject)
            {
                stopped = true;
            }
        }
    }
}
