using ColorManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.ColorSystems
{
    /// <summary>
    /// A system of colors, harmonizing with each other.
    /// </summary>
    public interface IColorSystem
    {
        /// <summary>
        /// The number of colors this system controls.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get or set a single color of this system. If one of the colors is set, the others are changed accordingly.
        /// The first color is always the "main" one, the most important color in the set. The "value" channel of all
        /// the colors in a system must always be the same.
        /// </summary>
        /// <param name="colorNumber">The number of the color to get/set</param>
        /// <returns>The chosen color.</returns>
        ColorHSV this[int colorNumber]
        {
            get;
            set;
        }
    }
}
