using ColorManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.ColorSystems
{
    /// <summary>
    /// A factory for IColorSystem instances.
    /// </summary>
    public static class ColorSystemFactory
    {
        /// <summary>
        /// Create an instance of a color system.
        /// </summary>
        /// <param name="kind">The kind of color system to create.</param>
        /// <param name="mainColor">The main color of this system.</param>
        /// <returns>The created color system.</returns>
        public static IColorSystem Create(ColorSystemKind kind, ColorHSV mainColor)
        {
            switch (kind)
            {
                case ColorSystemKind.AnalogousThree:
                    return new AnalogousThreeColorSystem(mainColor);
                case ColorSystemKind.AnalogousFive:
                    return new AnalogousFiveColorSystem(mainColor);
                case ColorSystemKind.Triad:
                    return new TriadColorSystem(mainColor);
                default:
                    throw new ArgumentException("kind");
            }
        }
    }
}
