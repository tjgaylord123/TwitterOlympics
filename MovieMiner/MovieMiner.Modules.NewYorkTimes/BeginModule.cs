using System;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MovieMiner.Interfaces.Modules;
using MovieMiner.Interfaces.Storage;

namespace MovieMiner.Modules.NewYorkTimes
{
    [Export(typeof(IDataModule))]
    public class BeginModule : IDataModule
    {
        private const string BaseAddress =
            "http://api.nytimes.com",
            APIKey = "e20cce9eeea18167a0f5d780a706ba12:0:69143363",
            ServiceProviderName = "NewYorkTimes";

        private IStorageClient _storageClient;
        private HttpClient _apiClient;

        public async Task StartModule(IStorageClient storageClient)
        {
            _storageClient = storageClient;
            _apiClient = new HttpClient {BaseAddress = new Uri(BaseAddress)};
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Get the first date we should query against
            DateTime date = storageClient.GetLatestDateFolderInStorage();

            while (date <= DateTime.Today.Date)
            {
                var response =
                    await _apiClient.GetAsync(
                            string.Format("/svc/movies/v2/reviews/search.json?opening-date={0}-{1}-{2}&api-key={3}",
                            date.Year,date.Month < 10 ? string.Format("0{0}", date.Month) : Convert.ToString(date.Month), 
                            date.Day < 10 ? string.Format("0{0}", date.Day) : Convert.ToString(date.Day), APIKey));
                if (response.IsSuccessStatusCode)
                {
                    string jsonBody = await response.Content.ReadAsStringAsync();
                    await storageClient.WriteFileToStorageAsync(jsonBody, date.Date, FileType.Json);
                }
                date = date.AddDays(1);
            }
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
