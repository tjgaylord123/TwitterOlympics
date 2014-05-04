using System.Collections.Generic;

namespace MovieAnalyzer.Interfaces.Modules.NewYorkTimes
{
    public class NYTQuery
    {
        public string status { get; set; }
        public string copyright { get; set; }
        public string num_results { get; set; }
        public List<Results> results { get; set; }
    }

    public class Results
    {
        public string nyt_movie_id { get; set; }

        public string thousand_best { get; set; }

        public string publication_date { get; set; }
    }

}
