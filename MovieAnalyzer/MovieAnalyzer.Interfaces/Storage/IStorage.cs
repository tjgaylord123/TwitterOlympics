using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieAnalyzer.Interfaces.Storage
{
    public interface IStorage
    {
        string GetFileContents(string fileName);
    }
}
