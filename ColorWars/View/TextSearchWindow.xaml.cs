using ColorWars.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ColorWars.View
{
    /// <summary>
    /// Interaction logic for TextSearchWindow.xaml
    /// </summary>
    public partial class TextSearchWindow : Window
    {
        public TextSearchWindow()
        {
            InitializeComponent();

            IsVisibleChanged += TextSearchWindow_IsVisibleChanged;
        }


        /// <summary>
        /// The framework element for which this is a search window.
        /// </summary>
        public FrameworkElement ReferenceFrameworkElement
        {
            get;
            set;
        }

        
        void TextSearchWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // animation for visible/invisible
            var textSearcher = (TextSearcher)App.Current.Resources["TextSearcher"];
            if (textSearcher.Searching)
                VisualStateManager.GoToElementState(MainTextBox, "Visible", true);
            else
                VisualStateManager.GoToElementState(MainTextBox, "Invisible", true);
            // from http://stackoverflow.com/a/3117138
            if (Visibility == System.Windows.Visibility.Visible)
                Dispatcher.BeginInvoke((Action)delegate
                {
                    // set focus on the text book
                    Keyboard.Focus(MainTextBox);
                    MainTextBox.CaretIndex = MainTextBox.Text.Length;
                    // move to the top right corner of the reference element
                    var ddp = ReferenceFrameworkElement;
                    if (ddp == null)
                        return;
                    var origin = ddp.PointToScreen(new Point());
                    Left = origin.X + ddp.ActualWidth - ActualWidth - 5;
                    Top = origin.Y + 5;
                }, DispatcherPriority.Render);
        }
    }
}
