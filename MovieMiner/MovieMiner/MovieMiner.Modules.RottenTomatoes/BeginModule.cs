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

        public void StartModule<TStorage>(string directory) where TStorage : IStorageClient
        {
            // Rotten Tomatoes does not support querying by date, so query by alphabetical letter for now...
            char[] allLetters = new char[26];
            char letterChar = 'A';
            for (int i = 0; i < 26; i++)
            {
                allLetters[i] = letterChar;
                letterChar++;
            }

            Parallel.ForEach(allLetters, letter =>
            {
                HttpClient httpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var queryTask = httpClient.GetAsync(
                        string.Format("api/public/v1.0/movies.json?apikey={0}&q={1}", APIKey, letter));
                queryTask.Wait();
                if (!queryTask.Result.IsSuccessStatusCode) return;

                RTQuery rtQuery = null;
                try
                {
                    var stringTask = queryTask.Result.Content.ReadAsStringAsync();
                    stringTask.Wait();
                    string text = (stringTask.Result).Trim();
                    rtQuery = JsonConvert.DeserializeObject<RTQuery>(text);
                }
                catch
                {
                } // Do nothing for now

                if (rtQuery != null && rtQuery.Movies != null && rtQuery.Movies.Count > 0)
                {
                    HttpClient innerHttpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
                    innerHttpClient.DefaultRequestHeaders.Accept.Clear();
                    innerHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    IStorageClient storageClient = (IStorageClient)Activator.CreateInstance(typeof(TStorage), new object[] { directory });
                    foreach(var movie in rtQuery.Movies)
                    {
                        var task = innerHttpClient.GetAsync(string.Format("api/public/v1.0/movies/{0}.json?apikey={1}", movie.id, APIKey));
                        task.Wait();
                        if (task.Result.IsSuccessStatusCode)
                        {
                            var thirdTask = task.Result.Content.ReadAsStringAsync();
                            thirdTask.Wait();
                            var innerTask = storageClient.WriteFileToStorageAsync(thirdTask.Result.Trim(), movie.title, FileType.Json);
                            innerTask.Wait();
                        }
                    }
                    innerHttpClient.Dispose();
                    storageClient.Dispose();
                }
                httpClient.Dispose();
            });

        }

        public string ModuleName
        {
            get { return ServiceProviderName; }
        }

    }
}
