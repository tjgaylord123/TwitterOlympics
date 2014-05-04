using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MovieAnalyzer.Interfaces.Modules;
using MovieAnalyzer.Interfaces.Modules.RottenTomatoes;
using MovieAnalyzer.Interfaces.Storage;
using MovieMiner.Interfaces.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieAnalyzer.Modules.RottenTomatoes
{
    [Export(typeof(IJobModule))]
    public class GenreAnalyzerJob : IJobModule
    {
        private IStorageClient _storageClient;
        private Dictionary<string, int> _genreCountDictionary; 

        public async Task BeginJobModule(IStorageClient storageClient)
        {
            _storageClient = storageClient;
            _genreCountDictionary = new Dictionary<string, int>();

            // Get every file name in the main directory
            object lockObject = new object();
            string[] allFileNames = _storageClient.GetAllFileNames();
            Parallel.ForEach(allFileNames, async fileName =>
            {
                string text = (await _storageClient.GetFileContents(fileName, FileType.Json)).Trim();
                if (String.IsNullOrEmpty(text)) return;

                Movie movie = null;
                try
                {
                    movie = JsonConvert.DeserializeObject<Movie>(text);
                }
                catch { }// Just continue for now

                if (movie == null || movie.genres == null || movie.genres.Count == 0) return;

                lock (lockObject)
                {
                    movie.genres.ForEach(genre =>
                    {
                        if (!_genreCountDictionary.ContainsKey(genre))
                        {
                            _genreCountDictionary.Add(genre, 0);
                        }
                        _genreCountDictionary[genre]++;
                    });
                }
            });

            JObject json = new JObject();
            foreach (var kv in _genreCountDictionary)
            {
                json.Add(kv.Key, new JValue(kv.Value));
            }

            await _storageClient.WriteFileToStorageAsync(json.ToString(), "genre_analysis", FileType.Json, true);
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _storageClient == null) return;

            _storageClient.Dispose();
            _storageClient = null;
        }

        #endregion

    }
}
