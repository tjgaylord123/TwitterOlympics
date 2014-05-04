using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MovieAnalyzer.Interfaces.Modules;
using MovieAnalyzer.Interfaces.Storage;

namespace MovieAnalyzer.Modules.RottenTomatoes
{
    [Export(typeof(IJobModule))]
    public class BeginModule : IJobModule
    {

        public Task BeginJobModule(IStorage storageClient)
        {
            throw new NotImplementedException();
        }

        public string ModuleName
        {
            get { return "RottenTomatoes"; }
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dipsosing)
        {
            
        }

        #endregion

    }
}
