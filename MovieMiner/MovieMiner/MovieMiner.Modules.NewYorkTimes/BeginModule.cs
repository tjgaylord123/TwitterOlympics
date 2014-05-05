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
        private static readonly DateTime _minDate = new DateTime(2000, 1, 1);
        private const string BaseAddress =
            "http://api.nytimes.com",
            APIKey = "e20cce9eeea18167a0f5d780a706ba12:0:69143363",
            ServiceProviderName = "NewYorkTimes";

        public void StartModule<TStorage>(string directory) where TStorage : IStorageClient
        {
            // Get the first date we should query against
            int allYears = DateTime.Today.Year - _minDate.Year;
            DateTime[] allDates = new DateTime[allYears*365];
            allDates[0] = _minDate;

            for (int i = 1; i < allYears; i++)
            {
                allDates[i] = allDates[i - 1].AddDays(1);
            }

            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            IStorageClient storageClient =
                (IStorageClient)Activator.CreateInstance(typeof (TStorage), new object[] {directory});

            HttpClient closureSafeHttpClient = httpClient;
            IStorageClient cosureSafeStorageClient = storageClient;

            Parallel.ForEach(allDates, date =>
            {
                try
                {
                    Task<HttpResponseMessage> queryResponse = closureSafeHttpClient.GetAsync(
                            string.Format("/svc/movies/v2/reviews/search.json?opening-date={0}-{1}-{2}&api-key={3}",
                                date.Year,
                                date.Month < 10 ? string.Format("0{0}", date.Month) : Convert.ToString(date.Month),
                                date.Day < 10 ? string.Format("0{0}", date.Day) : Convert.ToString(date.Day), APIKey));
                    queryResponse.Wait();
                    if (!queryResponse.Result.IsSuccessStatusCode) return;

                    Task<string> jsonBody = queryResponse.Result.Content.ReadAsStringAsync();
                    jsonBody.Wait();

                    Task<bool> writeTask = cosureSafeStorageClient.WriteFileToStorageAsync(jsonBody.Result, date.Date, FileType.Json);
                    writeTask.Wait();
                }
                catch { } // do nothing for now
            });

            httpClient.Dispose();
            storageClient.Dispose();
        }

        public string ModuleName
        {
            get { return ServiceProviderName; }
        }

    }
}
