using System.Collections.Generic;

namespace MovieAnalyzer.Interfaces.Modules.RottenTomatoes
{
    public class Movie
    {
        public string id { get; set; }

        public List<string> genres { get; set; } 
    }
}
