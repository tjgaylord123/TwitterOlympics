using System;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MovieMiner.Interfaces.Modules;
using MovieMiner.Interfaces.Storage;

namespace MovieMiner.Modules.RottenTomatoes
{
    [Export(typeof(IDataModule))]
    public class BeginModule : IDataModule
    {
        private const string
            BaseAddress = "http://api.rottentomatoes.com",
            ServiceProviderName = "RottenTomatoes",
            APIKey = "w4aruzts82vjkb5qagdufhq3";

        private IStorageClient _storageClient;
        private HttpClient _apiClient;

        public async Task StartModule(IStorageClient storageClient)
        {
            _storageClient = storageClient;
            _apiClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Rotten Tomatoes does not support querying by date, so query by alphabetical letter for now...
            await new Task(() => {});
        }

        public string ModuleName
        {
            get { return ServiceProviderName; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_apiClient != null)
            {
                _apiClient.Dispose();
                _apiClient = null;
            }
            if (_storageClient == null) return;

            _storageClient.Dispose();
            _storageClient = null;
        }
    }
}
