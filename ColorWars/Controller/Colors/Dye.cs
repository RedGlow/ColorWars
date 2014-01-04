using ColorManagment;
using ColorWars.Controller.Prices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.Colors
{
    /// <summary>
    /// A Guild Wars 2 Dye
    /// </summary>
    public class Dye: INotifyPropertyChanged
    {
        /// <summary>
        /// Numeric code of this color.
        /// </summary>
        public int Code { get; private set; }
        
        /// <summary>
        /// Name of the dye.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Base color.
        /// </summary>
        public ColorRGB BaseColor { get; private set; }

        /// <summary>
        /// Cloth variant of the color.
        /// </summary>
        public ColorRGB ClothColor { get; private set; }

        /// <summary>
        /// Leather variant of the color.
        /// </summary>
        public ColorRGB LeatherColor { get; private set; }

        /// <summary>
        /// Metal variant of the color.
        /// </summary>
        public ColorRGB MetalColor { get; private set; }

        /// <summary>
        /// The parent DyeSet this Dye is in.
        /// </summary>
        private DyeSet parent;

        /// <summary>
        /// Whether we already asked for the price.
        /// </summary>
        private bool priceAsked = false;

        /// <summary>
        /// The price data about this dye.
        /// </summary>
        public Price Price
        {
            get
            {
                if (!priceAsked)
                {
                    priceAsked = true;
                    new PriceFiller(this);
                }
                return price;
            }
            set
            {
                price = value;
                var pc = PropertyChanged;
                if (pc != null)
                {
                    pc(this, new PropertyChangedEventArgs("Price"));
                    pc(this, new PropertyChangedEventArgs("LoadingPrice"));
                    pc(this, new PropertyChangedEventArgs("PriceLoaded"));
                }
            }
        }
        private Price price = null;

        /// <summary>
        /// Whether the system is still loading the dye price.
        /// </summary>
        public bool LoadingPrice { get { return price == null; } }

        /// <summary>
        /// Whether the system has already loaded the dye price.
        /// </summary>
        public bool PriceLoaded { get { return price != null; } }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Create a new dye.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="baseColor"></param>
        /// <param name="clothColor"></param>
        /// <param name="leatherColor"></param>
        /// <param name="metalColor"></param>
        public Dye(DyeSet parent, string name, int code, ColorRGB baseColor, ColorRGB clothColor, ColorRGB leatherColor, ColorRGB metalColor)
        {
            this.parent = parent;
            Name = name;
            Code = code;
            BaseColor = baseColor;
            ClothColor = clothColor;
            LeatherColor = leatherColor;
            MetalColor = metalColor;
            var cc = new ColorConverter();
            var lab = cc.ToLab(MetalColor);
        }

        /// <summary>
        /// Returns a ColorRGB based on the material.
        /// </summary>
        /// <param name="material">Material currently used.</param>
        /// <returns>color for this dye on given material.</returns>
        public ColorRGB GetColor(Material material)
        {
            if (material == Material.Cloth)
                return ClothColor;
            else if (material == Material.Leather)
                return LeatherColor;
            else if (material == Material.Metal)
                return MetalColor;
            else
                throw new ArgumentException("material");
        }

        /* SORTING FIELDS */

        public double Hue { get { return getColorHSV().H; } }
        public double Saturation { get { return getColorHSV().S; } }
        public double Value { get { return getColorHSV().V; } }
        public double DistanceFromReference { get { return Distance(parent.ReferenceColor); } }

        private ColorHSV getColorHSV()
        {
            var colorSRGB = GetColor(parent.CurrentMaterial);
            var colorHSV = new ColorConverter().ToHSV(colorSRGB);
            return colorHSV;
        }

        /* DISTANCE */

        public double Distance(System.Windows.Media.Color otherColor)
        {
            // var the LAB colors
            var cc = new ColorConverter();
            var color1 = cc.ToLab(GetColor(parent.CurrentMaterial));
            var color2 = cc.ToLab(new ColorRGB(otherColor.R, otherColor.G, otherColor.B));

            // computer the difference using the CIEDE2000 algorithm
            // http://en.wikipedia.org/wiki/Color_difference#CIEDE2000
            const double kC = 1.0, kH = 1.0, kL = 1.0;
            var dLp = color2.L - color1.L;
            var Ls = (color1.L + color2.L) / 2;
            var C1s = Math.Sqrt(color1.a * color1.a + color1.b * color1.b);
            var C2s = Math.Sqrt(color2.a * color2.a + color2.b * color2.b);
            var Cs = (C1s + C2s) / 2;
            var p7 = 1 - Math.Sqrt(Math.Pow(Cs, 7) / (Math.Pow(Cs, 7) + Math.Pow(25, 7)));
            var a1p = color1.a + color1.a / 2 * p7;
            var a2p = color2.a + color2.a / 2 * p7;
            var C1p = Math.Sqrt(a1p * a1p + color1.b * color1.b);
            var C2p = Math.Sqrt(a2p * a2p + color2.b * color2.b);
            var Cps = (C1p + C2p) / 2;
            var dCp = C2p - C1p;
            var h1p = color1.b == 0 && a1p == 0 ? 0 : normalize(Math.Atan2(color1.b, a1p) * 180 / Math.PI);
            var h2p = color2.b == 0 && a2p == 0 ? 0 : normalize(Math.Atan2(color2.b, a2p) * 180 / Math.PI);
            var dhp = Math.Abs(h1p - h2p) <= 180 ? h2p - h1p :
                h2p <= h1p ? h2p - h1p + 360 :
                h2p - h1p - 360;
            var dHp = 2 * Math.Sqrt(C1p * C2p) * Math.Sin(dhp * Math.PI / 180 / 2);
            var Hps = C1p == 0 || C2p == 0 ? h1p + h2p : Math.Abs(h1p - h2p) > 180 ? (h1p + h2p + 360) / 2 : (h1p + h2p) / 2;
            var T = 1 - 0.17 * Math.Cos((Hps - 30) * Math.PI / 180) +
                0.24 * Math.Cos(2 * Hps * Math.PI / 180) +
                0.32 * Math.Cos((3 * Hps + 6) * Math.PI / 180) -
                0.20 * Math.Cos((4 * Hps - 63) * Math.PI / 180);
            var Lsm50sq = Sqr(Ls - 50);
            var SL = 1 + 0.015 * Lsm50sq / Math.Sqrt(20 + Lsm50sq);
            var SC = 1 + 0.045 * Cps;
            var SH = 1 + 0.015 * Cps * T;
            var RT = -1 * Math.Sqrt(Math.Pow(Cps, 7) / (Math.Pow(Cps, 7) + Math.Pow(25, 7))) *
                Math.Sin((60 * Math.Exp(- Sqr((Hps - 275) / 25))) * Math.PI / 180);
            var DEs00 = Math.Sqrt(
                Sqr(dLp / (kL * SL)) +
                Sqr(dCp / (kC * SC)) +
                Sqr(dHp / (kH * SH)) +
                RT * dCp * dHp / (kC * SC * kH * SH));

            return DEs00;
        }

        private double normalize(double degreeAngle)
        {
            while (degreeAngle < 0)
                degreeAngle += 360.0;
            while (degreeAngle > 360.0)
                degreeAngle -= 360.0;
            return degreeAngle;
        }

        private static double Sqr(double x)
        {
            return x * x;
        }
    }
}
