using ColorManagment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.ColorHarmonizer
{
    /// <summary>
    /// An arrow (color point) in the diagram of a color system.
    /// </summary>
    public class Arrow: INotifyPropertyChanged
    {
        /// <summary>
        /// Whether this arrow is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return color != null;
            }
        }

        /// <summary>
        /// The degree angle of this arrow in the HSV wheel.
        /// </summary>
        public double Angle {
            get
            {
                return color != null ? color.H : 0;
            }
            set
            {
                if (color == null)
                    return;
                value = ColorWars.Model.ColorSystems.Helper.NormalizeAngle(value);
                var newColor = new ColorHSV(value, color.S, color.V);
                setColorAndNotify(newColor);
            }
        }

        /// <summary>
        /// The length of this arrow, as a [0.0-1.0] measure in the HSV wheel.
        /// </summary>
        public double NormalizedLength
        {
            get
            {
                return color != null ? color.S : 0;
            }
            set
            {
                if (color == null)
                    return;
                value = Math.Min(1, Math.Max(0, value));
                var newColor = new ColorHSV(color.H, value, color.V);
                setColorAndNotify(newColor);
            }
        }

        /// <summary>
        /// A system color corresponding to this color.
        /// </summary>
        public System.Windows.Media.Color Color
        {
            get
            {
                if (color == null)
                    return System.Windows.Media.Colors.Transparent;
                var colorRGB = colorConverter.ToRGB(color);
                return System.Windows.Media.Color.FromRgb(
                    (byte)(colorRGB.R * 255),
                    (byte)(colorRGB.G * 255),
                    (byte)(colorRGB.B * 255));
            }
            set
            {
                if (color == null)
                    return;
                var newColorRGB = new ColorRGB(value.R, value.G, value.B);
                var newColor = colorConverter.ToHSV(newColorRGB);
                setColorAndNotify(newColor);
            }
        }

        /// <summary>
        /// Set given color and launch notifies the necessary changes.
        /// </summary>
        /// <param name="newColor">The new color to set.</param>
        private void setColorAndNotify(ColorHSV newColor)
        {
            if (color != newColor)
            {
                var oldColor = color;
                color = newColor;
                arrowsColorSetter.SetColor(index, newColor);
                sendChanged(oldColor, newColor);
            }
        }

        /// <summary>
        /// The color corresponding to this arrow, or null if it's disabled.
        /// </summary>
        private ColorHSV color;

        /// <summary>
        /// The color converter used to transform a color from HSV to a system color.
        /// </summary>
        private static ColorConverter colorConverter = new ColorConverter();

        /// <summary>
        /// Internal class to allow the parent to set the color on the child.
        /// </summary>
        private class ArrowColorSetter: IArrowColorSetter
        {
            private Arrow parent;
            public ArrowColorSetter(Arrow parent)
            {
                this.parent = parent;
            }
            public void SetColor(ColorHSV newColor)
            {
                var oldColor = parent.color;
                parent.color = newColor;
                parent.sendChanged(oldColor, newColor);
            }
        }


        /// <summary>
        /// Send PropertyChanged events according to what changed in the colors.
        /// </summary>
        /// <param name="oldColor">The old color.</param>
        /// <param name="newColor">The new color.</param>
        private void sendChanged(ColorHSV oldColor, ColorHSV newColor)
        {
            if (oldColor == newColor)
                return;
            var pe = PropertyChanged;
            if (pe == null)
                return;
            pe(this, new PropertyChangedEventArgs("Color"));
            if ((oldColor == null) != (newColor == null))
            {
                pe(this, new PropertyChangedEventArgs("Enabled"));
                pe(this, new PropertyChangedEventArgs("Angle"));
                pe(this, new PropertyChangedEventArgs("NormalizedLength"));
                return;
            }
            if (oldColor.H != newColor.H)
            {
                pe(this, new PropertyChangedEventArgs("Angle"));
            }
            if (oldColor.S != newColor.S)
            {
                pe(this, new PropertyChangedEventArgs("NormalizedLength"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The parent Arrows object.
        /// </summary>
        private Arrows parent;

        /// <summary>
        /// Index of this arrow in his parent object.
        /// </summary>
        private int index;

        /// <summary>
        /// Base-1 index of this arrow.
        /// </summary>
        public int BaseOneIndex { get { return index + 1;  } }

        /// <summary>
        /// Interface to set colors in the Arrows class.
        /// </summary>
        private IArrowsColorSetter arrowsColorSetter;

        /// <summary>
        /// Create a new Arrow object.
        /// </summary>
        /// <param name="parent">The parent (container) of this object.</param>
        /// <param name="arrowsColorSetter">Interface to set the colors in the parent object.</param>
        /// <param name="index">Index of this arrow in his parent.</param>
        /// <param name="color">The represented color.</param>
        public Arrow(Arrows parent, IArrowsColorSetter arrowsColorSetter, int index, ColorHSV color)
        {
            this.parent = parent;
            this.arrowsColorSetter = arrowsColorSetter;
            this.index = index;
            this.color = color;
            this.parent.SetArrowColorSetter(index, new ArrowColorSetter(this));
        }
    }
}
