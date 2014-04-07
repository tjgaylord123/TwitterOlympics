using System.ServiceProcess;
using System.Threading;
using MovieMiner.DiscoveryHost;

namespace MovieMiner.WindowsService
{
    public partial class ScheduledService : ServiceBase
    {
        public ScheduledService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            while (true)
            {
                DiscoverModules discoverModules = new DiscoverModules();
                discoverModules.Start();
                Thread.Sleep(86400000);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
