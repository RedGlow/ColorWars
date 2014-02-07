using ColorManagment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ColorWars.Controller.Colors
{
    /// <summary>
    /// Collection of all the dyes in the 
    /// </summary>
    public class DyeSet: DependencyObject
    {
        /// <summary>
        /// All the dyes in the game.
        /// </summary>
        public ObservableCollection<Dye> Dyes { get; private set; }

        /// <summary>
        /// All the dyes, sorted accorded to current algorithm.
        /// </summary>
        public ICollectionView SortedDyes { get; private set; }

        /// <summary>
        /// The current material to display and use
        /// </summary>
        public Material CurrentMaterial
        {
            get { return (Material)GetValue(CurrentMaterialProperty); }
            set { SetValue(CurrentMaterialProperty, value); }
        }

        public static readonly DependencyProperty CurrentMaterialProperty =
            DependencyProperty.Register("CurrentMaterial", typeof(Material), typeof(DyeSet), new PropertyMetadata(Material.Cloth));


        /// <summary>
        /// Sort order of the dyes
        /// </summary>
        public SortKind Sort
        {
            get { return (SortKind)GetValue(SortProperty); }
            set { SetValue(SortProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Sort.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortProperty =
            DependencyProperty.Register("Sort", typeof(SortKind), typeof(DyeSet),
                new PropertyMetadata(SortKind.Code, new PropertyChangedCallback(sortPropertyChanged)));

        private static void sortPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // get object
            var dyeSet = (DyeSet)d;

            // check whether we need a color picker
            dyeSet.NeedColorPicker = dyeSet.Sort == SortKind.NearestTo;

            // compute the new sorting criteria
            string property;
            ListSortDirection direction = ListSortDirection.Ascending;
            switch(dyeSet.Sort)
            {
                case SortKind.Hue:
                    property = "Hue";
                    break;
                case SortKind.Saturation:
                    property = "Saturation";
                    break;
                case SortKind.Value:
                    property = "Value";
                    break;
                case SortKind.Alphabetical:
                    property = "Name";
                    break;
                case SortKind.NearestTo:
                    property = "DistanceFromReference";
                    break;
                case SortKind.Code:
                default:
                    property = "Code";
                    break;
            }

            // set it
            using (dyeSet.SortedDyes.DeferRefresh())
            {
                dyeSet.SortedDyes.SortDescriptions.Clear();
                dyeSet.SortedDyes.SortDescriptions.Add(new SortDescription(property, direction));
            }
        }


        /// <summary>
        /// Whether a color picker must be displayed for the "NearestTo" sorting choice.
        /// </summary>
        public bool NeedColorPicker
        {
            get { return (bool)GetValue(NeedColorPickerProperty); }
            set { SetValue(NeedColorPickerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NeedColorPicker.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeedColorPickerProperty =
            DependencyProperty.Register("NeedColorPicker", typeof(bool), typeof(DyeSet), new PropertyMetadata(false));


        /// <summary>
        /// Reference color, other colors should measure their distance from.
        /// </summary>
        public System.Windows.Media.Color ReferenceColor
        {
            get { return (System.Windows.Media.Color)GetValue(ReferenceColorProperty); }
            set { SetValue(ReferenceColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReferenceColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReferenceColorProperty =
            DependencyProperty.Register("ReferenceColor", typeof(System.Windows.Media.Color), typeof(DyeSet),
            new PropertyMetadata(System.Windows.Media.Colors.Black, referenceColorPropertyChanged));

        private static void referenceColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // force refresh of the collection
            var dyeSet = (DyeSet)d;
            dyeSet.SortedDyes.Refresh();
        }


        /// <summary>
        /// The currently selected dye.
        /// </summary>
        public Dye CurrentlySelectedDye
        {
            get { return (Dye)GetValue(CurrentlySelectedDyeProperty); }
            set { SetValue(CurrentlySelectedDyeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentlySelectedDye.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentlySelectedDyeProperty =
            DependencyProperty.Register("CurrentlySelectedDye", typeof(Dye), typeof(DyeSet), new PropertyMetadata(null));
        

        
        /// <summary>
        /// String displayed while downloading / converting the dye data
        /// </summary>
        private const string workStatus = "Downloading dyes...";

        public DyeSet()
        {
            // initialize the color converter
            ColorConverter.Init();

            // create a new collection
            Dyes = new ObservableCollection<Dye>();

            // create a new view on that collection
            SortedDyes = CollectionViewSource.GetDefaultView(Dyes);

            // listen to changes to the currently searched color
            var dpd = DependencyPropertyDescriptor.FromProperty(TextSearcher.CurrentlySearchedStringProperty, typeof(TextSearcher));
            try
            {
                dpd.AddValueChanged(App.Current.Resources["TextSearcher"], new EventHandler(currentlySearchTextChanged));
            }
            catch (Exception)
            {
                // this is horrible, but the call in design mode on App.xml raises error, I don't know which error,
                // there's no way to know, and in this context it's impossible to determine if we are in
                // design mode because, hey, 2014, why should you be able to recognize such a simple thing?
            }

            // start the download of the dyes
            var downloader = new Model.Gw2api.Downloader();
            WorkStatus.Instance.SetStatus(workStatus);
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            downloader.Run().ContinueWith((task) =>
            {
                Debug.WriteLine("Converting dye list.");
                // convert the dyes data
                var dyeList = new List<Dye>();
                var colors = downloader.Result.colors;
                foreach (var key in colors.Keys)
                {
                    var code = int.Parse(key);
                    var color = colors[key];
                    Dyes.Add(new Dye(
                        this,
                        color.name,
                        code,
                        new ColorRGB(color.base_rgb[0], color.base_rgb[1], color.base_rgb[2]),
                        new ColorRGB(color.cloth.rgb[0], color.cloth.rgb[1], color.cloth.rgb[2]),
                        new ColorRGB(color.leather.rgb[0], color.leather.rgb[1], color.leather.rgb[2]),
                        new ColorRGB(color.metal.rgb[0], color.metal.rgb[1], color.metal.rgb[2])
                    ));
                }
                // ready
                Debug.WriteLine("Resetting status.");
                WorkStatus.Instance.RemoveStatus(workStatus);
            }, taskScheduler);
        }

        private void currentlySearchTextChanged(object sender, EventArgs e)
        {
            // get out the value we are searching for
            var textSearcher = (TextSearcher)sender;
            var toSearch = textSearcher.CurrentlySearchedString.ToLower();
            if (toSearch == string.Empty)
                return;

            // find the dye with the same name, or the first one that contains it
            Dye found = null;
            foreach (Dye dye in SortedDyes)
            {
                var ldn = dye.Name.ToLower();
                if (ldn.Equals(toSearch))
                {
                    found = dye;
                    break;
                }
                else if (found == null && dye.Name.ToLower().Contains(toSearch))
                    found = dye;
            }

            // set the selection to the found dye is any
            if (found != null)
                CurrentlySelectedDye = found;
        }
    }
}
