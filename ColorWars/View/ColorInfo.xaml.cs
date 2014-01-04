using ColorWars.Controller.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorWars.View
{
    /// <summary>
    /// Interaction logic for ColorInfo.xaml
    /// </summary>
    public partial class ColorInfo : UserControl
    {
        public ColorInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called whenever it's needed to search the color.
        /// </summary>
        public ICommand SearchColorCommand
        {
            get { return (ICommand)GetValue(SearchColorCommandProperty); }
            set { SetValue(SearchColorCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchColorCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchColorCommandProperty =
            DependencyProperty.Register("SearchColorCommand", typeof(ICommand), typeof(ColorInfo), new PropertyMetadata(null));

        
    }
}
