using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.Gw2SpidyApi
{
    /// <summary>
    /// Returned value from a Gw2Spidy search query.
    /// </summary>
    class Results
    {
#pragma warning disable 649
        public int count;
        public int page;
        public int last_page;
        public int total;
        public Result[] results;
#pragma warning restore 649

        public Results()
        {
            results = new Result[1];
        }
    }
}
