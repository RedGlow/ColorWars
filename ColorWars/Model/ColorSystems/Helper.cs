using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.ColorSystems
{
    /// <summary>
    /// An helper class for color system.
    /// </summary>
    static class Helper
    {
        /// <summary>
        /// Normalize a degree angle, forcing it between 0 and 360.
        /// </summary>
        /// <param name="angle">The angle to normalize.</param>
        /// <returns>The normalized angle.</returns>
        public static double NormalizeAngle(double angle) {
            while(angle < 360)
                angle += 360;
            while(angle > 360)
                angle -= 360;
            return angle;
        }
    }
}
