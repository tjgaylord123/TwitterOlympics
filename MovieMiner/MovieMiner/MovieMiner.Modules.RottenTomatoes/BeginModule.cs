using System;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MovieMiner.Interfaces.Modules;
using MovieMiner.Interfaces.Storage;
using Newtonsoft.Json;

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
            char letter = 'A';
            while (letter <= 'Z')
            {
                var queryResponse =
                    await _apiClient.GetAsync(
                        string.Format("api/public/v1.0/movies.json?apikey={0}&q={1}", APIKey, letter));
                if (queryResponse.IsSuccessStatusCode)
                {
                    // Get a JObject so we can query against specific movies returned

                    RTQuery rtQuery = null;
                    try
                    {
                        string text = (await queryResponse.Content.ReadAsStringAsync()).Trim();
                        rtQuery = JsonConvert.DeserializeObject<RTQuery>(text);
                    }
                    catch { } // Do nothing for now

                    if (rtQuery != null && rtQuery.Movies != null && rtQuery.Movies.Count > 0)
                    {
                        Parallel.ForEach(rtQuery.Movies, movie =>
                        {
                            Movie closureSafeMovie = movie;
                            var task = Task.Run(async () =>
                            {
                                HttpResponseMessage movieResponse =
                                    await _apiClient.GetAsync(
                                        string.Format("api/public/v1.0/{0}.json?apikey={1}", closureSafeMovie.id, APIKey));
                                if (movieResponse.IsSuccessStatusCode)
                                {
                                    await
                                        _storageClient.WriteFileToStorageAsync(
                                            await movieResponse.Content.ReadAsStringAsync(), closureSafeMovie.title,
                                            FileType.Json);
                                }
                            });
                            task.Wait();
                        });
                    }
      
                }
                letter++;
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
