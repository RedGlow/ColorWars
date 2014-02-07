using ColorWars.Controller;
using ColorWars.Controller.ColorHarmonizer;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            initializeArrowControls();
            textSearchWindow = new TextSearchWindow();
            textSearchWindow.ReferenceFrameworkElement = DyeList;
            SearchColor = new SearchColorCommand(this);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            clipboardWatcher.DataContext = new ClipboardManager(this);
            base.OnSourceInitialized(e);
        }

        private TextSearchWindow textSearchWindow;

        protected override void OnClosed(EventArgs e)
        {
            textSearchWindow.Close();
            base.OnClosed(e);
        }

        private void DyeList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                var textSearcher = (TextSearcher)App.Current.Resources["TextSearcher"];
                char ch = (char)((e.Key - Key.A) + 'A');
                textSearcher.CurrentlySearchedString += ch;
                e.Handled = true;
            }
        }

        private void DyeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DyeList.ScrollIntoView(DyeList.SelectedItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        private ArrowControl[] arrowControls = null;

        private void initializeArrowControls()
        {
            arrowControls = new ArrowControl[5];
            arrowControls[0] = ArrowControl0;
            arrowControls[1] = ArrowControl1;
            arrowControls[2] = ArrowControl2;
            arrowControls[3] = ArrowControl3;
            arrowControls[4] = ArrowControl4;
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var arrowControl in arrowControls)
            {
                arrowControl.MyOnPreviewMouseLeftButtonDown(e);
                if (e.Handled)
                    break;
            }
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var arrowControl in arrowControls)
            {
                arrowControl.MyOnPreviewMouseLeftButtonUp(e);
                if (e.Handled)
                    break;
            }
        }

        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            foreach (var arrowControl in arrowControls)
            {
                arrowControl.MyOnPreviewMouseMove(e);
                if (e.Handled)
                    break;
            }
        }

        private class SearchColorCommand: ICommand
        {
            private MainWindow parent;

            public SearchColorCommand(MainWindow mainWindow)
            {
                this.parent = mainWindow;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged { add { } remove { } }

            public void Execute(object parameter)
            {
                var arrow = (Arrow)parameter;
                var ds = (DyeSet)App.Current.Resources["DyeSet"];
                ds.ReferenceColor = arrow.Color;
                ds.Sort = SortKind.NearestTo;
                parent.MainTabControl.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// The command to perform a color search.
        /// </summary>
        public ICommand SearchColor { get; private set; }
    }
}
