using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.ColorSystems
{
    /// <summary>
    /// The possible color system kinds.
    /// </summary>
    public enum ColorSystemKind
    {
        /// <summary>
        /// An analogous color system composed of three colors.
        /// </summary>
        [Description("Analogous (3 colors)")]
        AnalogousThree,
        /// <summary>
        /// An analogous color system compoed of five colors.
        /// </summary>
        [Description("Analogous (5 colors)")]
        AnalogousFive,
        /// <summary>
        /// A triad color system.
        /// </summary>
        [Description("Triad")]
        Triad
    }
}
