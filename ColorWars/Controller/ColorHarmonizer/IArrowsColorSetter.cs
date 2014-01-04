using ColorManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.ColorHarmonizer
{
    /// <summary>
    /// Interface used by Arrow to change the color in Arrows.
    /// </summary>
    public interface IArrowsColorSetter
    {
        /// <summary>
        /// Sets the specific color of an arrow.
        /// </summary>
        /// <param name="index">Index of the arrow.</param>
        /// <param name="newColor">New color of the arrow.</param>
        void SetColor(int index, ColorHSV newColor);
    }
}
