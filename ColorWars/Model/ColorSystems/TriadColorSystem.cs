using ColorManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.ColorSystems
{
    /// <summary>
    /// A color system where the three colors always stay at 120 degrees of distance, covering the color space equally.
    /// </summary>
    class TriadColorSystem: IColorSystem
    {
        public TriadColorSystem(ColorHSV mainColor)
        {
            colors = new ColorHSV[3];
            colors[0] = mainColor;
            computeColorsFrom(0);
        }

        /// <summary>
        /// The colors of this system.
        /// </summary>
        private ColorHSV[] colors;

        public int Count
        {
            get { return 3; }
        }

        public ColorHSV this[int colorNumber]
        {
            get
            {
                return colors[colorNumber];
            }
            set
            {
                colors[colorNumber] = value;
                computeColorsFrom(colorNumber);
            }
        }

        /// <summary>
        /// Compute the two other colors of a triad starting from the one of them.
        /// </summary>
        /// <param name="i">The index of the color to use as a base to compute the other two.</param>
        private void computeColorsFrom(int i)
        {
            var c = colors[i];
            for (var j = 1; j < 3; j++)
                colors[(i + j) % 3] = new ColorHSV(Helper.NormalizeAngle(c.H + j * 120), c.S, c.V);
        }
    }
}
