using System.Collections.Generic;

namespace MovieMiner.Interfaces.Modules
{
    public class RTQuery
    {
        public List<Movie> Movies { get; set; } 
    }

    public class Movie
    {
        public string id { get; set; }

        public string title { get; set; }
    }
}
