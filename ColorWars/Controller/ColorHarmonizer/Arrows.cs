using ColorManagment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.ColorHarmonizer
{
    /// <summary>
    /// A set of arrows (color points in the HSV wheel).
    /// </summary>
    public class Arrows: INotifyPropertyChanged
    {
        /// <summary>
        /// First arrow (main color).
        /// </summary>
        public Arrow Arrow0
        {
            get
            {
                return arrows[0];
            }
        }

        /// <summary>
        /// Second arrow.
        /// </summary>
        public Arrow Arrow1
        {
            get
            {
                return arrows[1];
            }
        }

        /// <summary>
        /// Third arrow.
        /// </summary>
        public Arrow Arrow2
        {
            get
            {
                return arrows[2];
            }
        }

        /// <summary>
        /// Whether this system has three arrows.
        /// </summary>
        public bool HasArrow2
        {
            get
            {
                return arrows.Length >= 3;
            }
        }

        /// <summary>
        /// Fourth arrow.
        /// </summary>
        public Arrow Arrow3
        {
            get
            {
                return arrows[3];
            }
        }

        /// <summary>
        /// Fifth arrow.
        /// </summary>
        public Arrow Arrow4
        {
            get
            {
                return arrows[4];
            }
        }

        /// <summary>
        /// The used color system kind.
        /// </summary>
        public Model.ColorSystems.ColorSystemKind ColorSystemKind
        {
            get
            {
                return colorSystemKind;
            }
            set
            {
                if (colorSystemKind != value)
                {
                    colorSystemKind = value;
                    colorSystem = Model.ColorSystems.ColorSystemFactory.Create(
                        colorSystemKind,
                        colorSystem[0]);
                    for (int i = 0; i < 5; i++)
                        setArrowFromColorSystem(i);
                    propChanged("ColorSystemKind");
                }
            }
        }

        private void propChanged(string property)
        {
            var e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// The value channel for all the colors in the system.
        /// </summary>
        public double Value
        {
            get
            {
                return value;
            }
            set
            {
                value = Math.Min(1, Math.Max(0, value));
                if (value != this.value)
                {
                    // value has changed
                    this.value = value;
                    var e = PropertyChanged;
                    if (e != null)
                        e(this, new PropertyChangedEventArgs("Value"));
                    // update one entry of the color system (by contract, all the other entries
                    // must change the value too)
                    colorSystem[0] = new ColorHSV(colorSystem[0].H, colorSystem[0].S, value);
                    // inform the Arrow controller objects of the change
                    for (int i = 0; i < arrows.Length; i++)
                        setArrowFromColorSystem(i);
                }
            }
        }

        private double value = 1.0;

        /// <summary>
        /// Set the value of an arrow from the internal color system.
        /// </summary>
        /// <param name="i">Index of the arrow to set</param>
        private void setArrowFromColorSystem(int i)
        {
            arrowColorSetters[i].SetColor(
                i < colorSystem.Count ?
                colorSystem[i] :
                null);
        }

        /// <summary>
        /// Internal color system kind.
        /// </summary>
        private Model.ColorSystems.ColorSystemKind colorSystemKind;

        /// <summary>
        /// Internal color system.
        /// </summary>
        private Model.ColorSystems.IColorSystem colorSystem;

        /// <summary>
        /// Internal array of arrows.
        /// </summary>
        private Arrow[] arrows;

        /// <summary>
        /// Interface used to allow an Arrow to set a color in this class.
        /// </summary>
        private class ArrowsColorSetter : IArrowsColorSetter
        {
            private Arrows parent;
            public ArrowsColorSetter(Arrows parent)
            {
                this.parent = parent;
            }
            public void SetColor(int index, ColorHSV newColor)
            {
                parent.colorSystem[index] = newColor;
                for (int i = 0; i < parent.colorSystem.Count; i++)
                    if (i != index)
                        parent.setArrowFromColorSystem(i);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Internal array of color setters for Arrow classes.
        /// </summary>
        private IArrowColorSetter[] arrowColorSetters;

        /// <summary>
        /// Set the IArrowColorSetter for an arrow.
        /// </summary>
        /// <param name="index">The index of the color setter.</param>
        /// <param name="arrowColorSetter">The color setter.</param>
        public void SetArrowColorSetter(int index, IArrowColorSetter arrowColorSetter)
        {
            arrowColorSetters[index] = arrowColorSetter;
        }

        private const int maxArrows = 5;

        public Arrows()
        {
            // initialize color system
            colorSystemKind = Model.ColorSystems.ColorSystemKind.AnalogousThree;
            colorSystem = Model.ColorSystems.ColorSystemFactory.Create(
                colorSystemKind,
                new ColorHSV(0, 1, 1));
            // initialize arrows
            arrows = new Arrow[maxArrows];
            arrowColorSetters = new IArrowColorSetter[maxArrows];
            var arrowsColorSetter = new ArrowsColorSetter(this);
            for (int i = 0; i < maxArrows; i++)
                arrows[i] = new Arrow(this, arrowsColorSetter, i,
                    i < colorSystem.Count ? colorSystem[i] : null);
        }
    }
}
