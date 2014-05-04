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
    public class ThousandsListAnalyzerJob : IJobModule
    {
        private IStorageClient _storageClient;
        private Dictionary<string, ThousandsList> _thousandsLists;

        public async Task BeginJobModule(IStorageClient storageClient)
        {
            _storageClient = storageClient;
            _thousandsLists = new Dictionary<string, ThousandsList>();

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
                    int yesNo;
                    if (!int.TryParse(movie.thousand_best, out yesNo)) continue;

                    DateTime date;
                    if (!DateTime.TryParse(movie.publication_date, out date)) continue;

                    string key = Convert.ToString(date.Date.Year);
                    lock (lockObject)
                    {
                        if (!_thousandsLists.ContainsKey(key))
                        {
                            _thousandsLists.Add(key, new ThousandsList
                            {
                                Yes = new Yes
                                {
                                    Count = 0
                                },
                                No = new No
                                {
                                    Count = 0
                                }
                            });
                        }
                        switch (yesNo)
                        {
                            case 0:
                                _thousandsLists[key].No.Count++;
                                break;
                            case 1:
                                _thousandsLists[key].Yes.Count++;
                                break;
                        }
                    }

                }
            });
            //foreach (string fileName in allFileNames)
            //{
            //    string text = (await _storageClient.GetFileContents(fileName, FileType.Json)).Trim();
            //    if (String.IsNullOrEmpty(text)) continue;

            //    NYTQuery movies = null;
            //    try
            //    {
            //        movies = JsonConvert.DeserializeObject<NYTQuery>(text);
            //    }
            //    catch
            //    {
            //    } // Just continue for now

            //    if (movies == null || movies.results == null || movies.results.Count == 0) continue;

            //    foreach (var movie in movies.results)
            //    {
            //        int yesNo;
            //        if (!int.TryParse(movie.thousand_best, out yesNo)) continue;

            //        DateTime date;
            //        if (!DateTime.TryParse(movie.publication_date, out date)) continue;

            //        string key = Convert.ToString(date.Date.Year);
            //        if (!_thousandsLists.ContainsKey(key))
            //        {
            //            _thousandsLists.Add(key, new ThousandsList
            //            {
            //                Yes = new Yes
            //                {
            //                    Count = 0
            //                },
            //                No = new No
            //                {
            //                    Count = 0
            //                }
            //            });
            //        }
            //        switch (yesNo)
            //        {
            //            case 0:
            //                _thousandsLists[key].No.Count++;
            //                break;
            //            case 1:
            //                _thousandsLists[key].Yes.Count++;
            //                break;
            //        }

            //    }
            //}


            string json = JsonConvert.SerializeObject(_thousandsLists, Formatting.Indented);
            await _storageClient.WriteFileToStorageAsync(json, "thousands_list", FileType.Json, true);
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

        private class ThousandsList
        {
            public Yes Yes { get; set; }
            public No No { get; set; }
        }

        private class Yes
        {
            public int Count { get; set; }
        }

        private class No
        {
            public int Count { get; set; }
        }

        #endregion
    }
}
