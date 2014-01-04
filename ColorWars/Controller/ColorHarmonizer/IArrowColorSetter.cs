using ColorManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.ColorHarmonizer
{
    /// <summary>
    /// Interface to allow setting a color.
    /// </summary>
    public interface IArrowColorSetter
    {
        /// <summary>
        /// Sets the color of this arrow. This method may be called only by the parent object.
        /// </summary>
        /// <param name="newColor">The new color.</param>
        void SetColor(ColorHSV newColor);
    }
}
