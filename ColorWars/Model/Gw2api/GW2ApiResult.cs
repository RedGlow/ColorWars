using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.Gw2api
{
    class GW2ApiResult
    {
        public Dictionary<string, GW2ApiColorEntry> colors;

        public GW2ApiResult()
        {
            colors = new Dictionary<string, GW2ApiColorEntry>();
        }
    }
}
