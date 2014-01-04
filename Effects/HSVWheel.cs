using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Effects
{
    /// <summary>
    /// A filter that superimposes an HSV wheel over the input image .
    /// </summary>
    public class HSVWheel: ShaderEffect
    {
        /// <summary>
        /// Pixel shader used by this effect
        /// </summary>
        private static PixelShader pixelShader = new PixelShader();

        /// <summary>
        /// Static initializer, used to create the pixel shader
        /// </summary>
        static HSVWheel()
        {
            pixelShader.UriSource = Global.MakePackUri("HSVWheel.ps");
        }


        /// <summary>
        /// Creates a new HSVWheel.
        /// </summary>
        public HSVWheel()
        {
            this.PixelShader = pixelShader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(ValueProperty);
        }


        /// <summary>
        /// The input image (associated with the image register 0).
        /// </summary>
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(HSVWheel), 0);


        /// <summary>
        /// The value channel (associated with the constant register 0).
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(HSVWheel), new UIPropertyMetadata(1.0d, PixelShaderConstantCallback(0)));
    }
}
