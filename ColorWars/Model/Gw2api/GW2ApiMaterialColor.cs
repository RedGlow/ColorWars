using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.Gw2api
{
    public class GW2ApiMaterialColor
    {
        public int brightness;
        public float contrast;
        public int hue;
        public float saturation;
        public float lightness;
        public byte[] rgb;

        public GW2ApiMaterialColor()
        {
            rgb = new byte[3];
        }
    }
}
