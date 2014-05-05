using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using MovieAnalyzer.Interfaces.Modules;
using MovieAnalyzer.Interfaces.Modules.NewYorkTimes;
using MovieAnalyzer.Interfaces.Storage;
using MovieMiner.Interfaces.Storage;
using Newtonsoft.Json;

namespace MovieAnalyzer.Modules.NewYorkTimes
{
    [Export(typeof(IJobModule))]
    public class MPAAAnalyzerJob : IJobModule
    {
        private IStorageClient _storageClient;
        private Dictionary<string, MPAA> _mpaaList;

        public async Task BeginJobModule(IStorageClient storageClient)
        {
            _storageClient = storageClient;
            _mpaaList = new Dictionary<string, MPAA>();

            // Get every file name in the main directory
            object lockObject = new object();
            string[] allFileNames = _storageClient.GetAllFileNames();
            Parallel.ForEach(allFileNames, async fileName =>
            {
                string text = (await _storageClient.GetFileContents(fileName, FileType.Json)).Trim();
                if (String.IsNullOrEmpty(text)) return;

                NYTQuery movies = null;
                try
                {
                    movies = JsonConvert.DeserializeObject<NYTQuery>(text);
                }
                catch
                {
                } // Just continue for now

                if (movies == null || movies.results == null || movies.results.Count == 0) return;

                foreach (var movie in movies.results)
                {

                    DateTime date;
                    if (!DateTime.TryParse(movie.publication_date, out date)) continue;

                    string key = Convert.ToString(date.Date.Year);
                    lock (lockObject)
                    {
                        if (!_mpaaList.ContainsKey(key))
                        {
                            _mpaaList.Add(key, new MPAA
                            {
                                G = new G {Count = 0},
                                PG = new PG {Count = 0},
                                PG13 = new PG13 {Count = 0},
                                R = new R {Count = 0},
                                None = new None {  Count = 0}
                            });
                        }
                        switch (movie.mpaa_rating)
                        {
                            case "G":
                                _mpaaList[key].G.Count++;
                                break;
                            case "PG":
                                _mpaaList[key].PG.Count++;
                                break;
                            case "PG13":
                                _mpaaList[key].PG13.Count++;
                                break;
                            case "R":
                                _mpaaList[key].R.Count++;
                                break;
                            default:
                                _mpaaList[key].None.Count++;
                                break;
                        }
                    }

                }
            });

            string json = JsonConvert.SerializeObject(_mpaaList, Formatting.Indented);
            await _storageClient.WriteFileToStorageAsync(json, "mpaa_analyzer", FileType.Json, true);
    }

        public string ModuleName
        {
            get { return "NewYorkTimes"; }
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

        #region Helpers

        private class MPAA
        {
            public G G { get; set; }

            public PG PG { get; set; }

            public PG13 PG13 { get; set; }

            public R R { get; set; }

            public None None { get; set; }
        }

        private class G
        {
            public int Count { get; set; }
        }

        private class PG
        {
            public int Count { get; set; }
        }

        private class PG13
        {
            public int Count { get; set; }
        }

        private class R
        {
            public int Count { get; set; }
        }

        private class None
        {
            public int Count { get; set; }
        }

        #endregion
    }
}
