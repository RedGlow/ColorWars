using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.Gw2api
{
    public class GW2ApiColorEntry
    {
        public string name;
        public byte[] base_rgb;
        public GW2ApiMaterialColor cloth;
        public GW2ApiMaterialColor leather;
        public GW2ApiMaterialColor metal;

        public GW2ApiColorEntry()
        {
            base_rgb = new byte[3];
        }
    }
}
