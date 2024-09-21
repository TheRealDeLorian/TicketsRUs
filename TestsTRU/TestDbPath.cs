using MauiTRU.Database;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsTRU
{
    public class TestDbPath : IDbPath
    {
        public string Directory => System.IO.Directory.GetCurrentDirectory();
    }
}
