using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ColorWars.View
{
    /// <summary>
    /// Data saved during the dragging of an arrow.
    /// </summary>
    class ArrowDraggingData
    {
        /// <summary>
        /// The starting value for the angle.
        /// </summary>
        public readonly double StartingAngle;

        /// <summary>
        /// The starting value for the normalized length.
        /// </summary>
        public readonly double StartingNormalizedLength;

        /// <summary>
        /// Coordinates of the click position, in the control reference system.
        /// </summary>
        public readonly Point ClickCoordinates;

        /// <summary>
        /// Angle of the click position, in degrees.
        /// </summary>
        public readonly double ClickAngle;

        /// <summary>
        /// Normalized length of the click position.
        /// </summary>
        public readonly double ClickNormalizedLength;

        /// <summary>
        /// Create a new ArrowDraggingData.
        /// </summary>
        /// <param name="startingAngle">The starting value for the angle.</param>
        /// <param name="startingNormalizedLength">The starting value for the normalized length.</param>
        /// <param name="clickCoordinates">Coordinates of the click position, in the control reference system.</param>
        /// <param name="centerCoordinates">Coordinates of the center of the control.</param>
        public ArrowDraggingData(double startingAngle,
            double startingNormalizedLength,
            Point clickCoordinates,
            Point centerCoordinates)
        {
            StartingAngle = startingAngle;
            StartingNormalizedLength = startingNormalizedLength;
            ClickCoordinates = clickCoordinates;
            var relativeClickCoordinates = new Point(clickCoordinates.X - centerCoordinates.X, clickCoordinates.Y - centerCoordinates.Y);
            ClickAngle = Math.Atan2(relativeClickCoordinates.Y, relativeClickCoordinates.X) * 180 / Math.PI;
            ClickNormalizedLength = Math.Sqrt(relativeClickCoordinates.X * relativeClickCoordinates.X +
                relativeClickCoordinates.Y * relativeClickCoordinates.Y) / centerCoordinates.X;
        }
    }
}
