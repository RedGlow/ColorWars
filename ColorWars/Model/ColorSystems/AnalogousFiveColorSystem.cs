using ColorManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.ColorSystems
{
    /// <summary>
    /// Creates an analogous color system. In this system, the main color is at the center, and the four lateral colors always stay
    /// at symmetric hue angles.
    /// </summary>
    class AnalogousFiveColorSystem: AnalogousBaseColorSystem
    {
        /// <summary>
        /// Creates a new AnalogousColorSystem, with a hue angle of 30 degrees.
        /// </summary>
        /// <param name="mainColor">The first color.</param>
        public AnalogousFiveColorSystem(ColorHSV mainColor)
            : base(mainColor, 5)
        {
        }
    }
}
