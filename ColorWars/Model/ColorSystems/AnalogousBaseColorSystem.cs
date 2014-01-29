using ColorManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.ColorSystems
{
    /// <summary>
    /// Creates an analogous color system. In this system, the main color is at the center, and the lateral colors always stay
    /// at symmetric hue angles.
    /// </summary>
    class AnalogousBaseColorSystem: IColorSystem
    {
        /// <summary>
        /// Creates a new AnalogousBaseColorSystem, with a hue angle of 30 degrees.
        /// </summary>
        /// <param name="mainColor">The first color.</param>
        /// <param name="count">The number of colors.</param>
        public AnalogousBaseColorSystem(ColorHSV mainColor, int count)
        {
            Count = count;
            colors = new ColorHSV[Count];
            colors[0] = mainColor;
            hueAngle = 30;
            computeLaterals();
        }

        public int Count { get; private set; }

        /// <summary>
        /// The colors.
        /// </summary>
        private ColorHSV[] colors;

        /// <summary>
        /// The angle between the main color and one of the adjacent lateral colors.
        /// </summary>
        private double hueAngle;

        public ColorHSV this[int colorNumber]
        {
            get
            {
                return colors[colorNumber];
            }
            set
            {
                if (colorNumber == 0)
                {
                    // central color has been moved: move it and compute laterals again
                    colors[0] = value;
                    computeLaterals();
                }
                else
                {
                    // lateral color has been moved: change the angle accordingly and compute the other lateral.
                    int factor = getFactor(colorNumber);
                    var diffAngle = value.H - colors[0].H;
                    var normalizedDiffAngle = diffAngle > 180 ? -(360 - diffAngle) : diffAngle;
                    hueAngle = normalizedDiffAngle / factor;
                    colors[0] = new ColorHSV(colors[0].H, value.S, value.V);
                    computeLaterals();
                }
            }
        }

        private static int getFactor(int colorNumber)
        {
            int df = (colorNumber + 1) / 2;
            int factor = (colorNumber % 2 == 0 ? -1 : 1) * df;
            return factor;
        }

        /// <summary>
        /// Compute lateral colors from main color.
        /// </summary>
        private void computeLaterals()
        {
            var c = colors[0];
            for (int i = 1; i < Count; i++)
                colors[i] = new ColorHSV(Helper.NormalizeAngle(getFactor(i) * hueAngle + c.H), c.S, c.V);
        }
    }
}
