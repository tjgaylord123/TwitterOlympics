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

            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            IStorageClient storageClient = (IStorageClient)Activator.CreateInstance(typeof(TStorage), new object[] { directory });

            HttpClient closureSafeHttpClient = httpClient;
            IStorageClient closureSafeStorageClient = storageClient;
            Parallel.ForEach(allLetters, async letter =>
            {
                Task<HttpResponseMessage> queryResponse;
                try
                {
                    queryResponse = closureSafeHttpClient.GetAsync(
                        string.Format("api/public/v1.0/movies.json?apikey={0}&q={1}", APIKey, letter));
                    queryResponse.Wait();
                }
                catch
                {
                    return;
                } // Do nothing for now

                if (!queryResponse.Result.IsSuccessStatusCode) return;

                RTQuery rtQuery;
                try
                {
                    string content = await queryResponse.Result.Content.ReadAsStringAsync();
                    rtQuery = JsonConvert.DeserializeObject<RTQuery>(content.Trim());
                }
                catch
                {
                    return;
                } // Do nothing for now

                if (rtQuery == null || rtQuery.Movies == null || rtQuery.Movies.Count <= 0) return;

                foreach(var movie in rtQuery.Movies)
                {
                    Task<HttpResponseMessage> innerQueryResponse;
                    try
                    {
                        innerQueryResponse = closureSafeHttpClient.GetAsync(
                            string.Format("api/public/v1.0/movies/{0}.json?apikey={1}", movie.id, APIKey));
                        innerQueryResponse.Wait();
                    }
                    catch
                    {
                        return;
                    }
                    if (!innerQueryResponse.Result.IsSuccessStatusCode) continue;

                    Task<string> innerContent = innerQueryResponse.Result.Content.ReadAsStringAsync();
                    innerContent.Wait();

                    try
                    {
                       Task<bool> writeTask = closureSafeStorageClient.WriteFileToStorageAsync(innerContent.Result.Trim(), movie.title,
                            FileType.Json);
                        writeTask.Wait();
                    }
                    catch
                    {
                        return;
                    }
                }
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
