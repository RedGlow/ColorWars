using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorWars.Controller.ColorHarmonizer;


namespace ColorWars.Controller
{
    public class ClipboardManager: INotifyPropertyChanged
    {
        #region constructors

        public ClipboardManager()
        {
            initializeCopyToHarmonizer();
        }


        public ClipboardManager(Window mainWindow)
        {
            if (mainWindow == null)
                throw new ArgumentNullException("mainWindow");
            this.mainWindow = mainWindow;
            initializeCopyToHarmonizer();
            hookEvents();
        }

        #endregion



        #region dependency properties

        /// <summary>
        /// The image currently contained in the clipboard, or <c>null</c> if there's no image in the
        /// clipboard.
        /// </summary>
        public ImageSource Image
        {
            get { return image; }
            set { image = value; propertyChanged("Image"); }
        }

        private ImageSource image = null;



        public const double MinimumZoomFactor = 0.1;

        public const double MaximumZoomFactor = 10;

        public const double ZoomDelta = 0.1;

        /// <summary>
        /// The currently used zoom factor.
        /// </summary>
        public double ZoomFactor
        {
            get { return zoomFactor; }
            set {
                if (value < MinimumZoomFactor)
                    value = MinimumZoomFactor;
                if (value > MaximumZoomFactor)
                    value = MaximumZoomFactor;
                zoomFactor = value;
                propertyChanged("ZoomFactor");
            }
        }

        private double zoomFactor = 1.0;

        public void IncreaseZoom()
        {
            ZoomFactor += ZoomDelta;
        }

        public void DecreaseZoom()
        {
            ZoomFactor -= ZoomDelta;
        }



        public const int MinimumPickerDiameter = 4;

        public const int MaximumPickerDiameter = 100;

        public const int PickerDiameterDelta = 1;

        /// <summary>
        /// Radius of the picker tool (ignores the zoom).
        /// </summary>
        public int PickerDiameter
        {
            get { return pickerDiameter; }
            set {
                if (value < MinimumPickerDiameter)
                    value = MinimumPickerDiameter;
                if (value > MaximumPickerDiameter)
                    value = MaximumPickerDiameter;
                if (pickerDiameter != value)
                {
                    pickerDiameter = value;
                    updatePickerTopLeftCornerPosition();
                    propertyChanged("PickerDiameter");
                }
            }
        }

        private int pickerDiameter = 20;

        public void IncreasePickerDiameter()
        {
            PickerDiameter += PickerDiameterDelta;
        }

        public void DecreasePickerDiameter()
        {
            PickerDiameter -= PickerDiameterDelta;
        }



        public static Point AwayPoint = new Point(double.PositiveInfinity, double.PositiveInfinity);

        /// <summary>
        /// Position of the picker inside the image (ignores the zoom).
        /// </summary>
        public Point PickerPosition
        {
            get { return pickerPosition; }
            set
            {
                if (pickerPosition != value)
                {
                    pickerPosition = value;
                    IsPickerVisible = PickerPosition != AwayPoint;
                    updatePickerTopLeftCornerPosition();
                    propertyChanged("PickerPosition");
                }
            }
        }

        private Point pickerPosition = AwayPoint;

        public void HidePicker()
        {
            PickerPosition = AwayPoint;
        }



        /// <summary>
        /// Position of the picker's top left corner (ignores the zoom).
        /// </summary>
        public Point PickerTopLeftCornerPosition
        {
            get { return pickerTopLeftCornerPosition; }
            set
            {
                if (pickerTopLeftCornerPosition != value)
                {
                    pickerTopLeftCornerPosition = value;
                    propertyChanged("PickerTopLeftCornerPosition");
                }
            }
        }

        private Point pickerTopLeftCornerPosition = AwayPoint;

        private void updatePickerTopLeftCornerPosition()
        {
            if (PickerPosition == AwayPoint)
                PickerTopLeftCornerPosition = AwayPoint;
            else
                PickerTopLeftCornerPosition = new Point(
                    PickerPosition.X - PickerDiameter / 2,
                    PickerPosition.Y - PickerDiameter / 2);
        }

        

        /// <summary>
        /// Whether the picker is currently visible.
        /// </summary>
        public bool IsPickerVisible
        {
            get { return isPickerVisible; }
            set
            {
                if (isPickerVisible != value)
                {
                    isPickerVisible = value;
                    propertyChanged("IsPickerVisible");
                }
            }
        }

        private bool isPickerVisible = false;



        /// <summary>
        /// The horizontal offset of the image (ignores the zoom).
        /// </summary>
        public double HorizontalOffset
        {
            get { return horizontalOffset; }
            set
            {
                if (horizontalOffset != value)
                {
                    horizontalOffset = value;
                    offsetChanged();
                    propertyChanged("HorizontalOffset");
                }
            }
        }

        private double horizontalOffset = 0.0;



        /// <summary>
        /// The vertical offset of the image (ignores the zoom).
        /// </summary>
        public double VerticalOffset
        {
            get { return verticalOffset; }
            set
            {
                if (verticalOffset != value)
                {
                    verticalOffset = value;
                    offsetChanged();
                    propertyChanged("VerticalOffset");
                }
            }
        }

        private double verticalOffset = 0.0;



        /// <summary>
        /// A margin to assign to an element with a (HorizontalOffset, VerticalOffset) top left margin.
        /// </summary>
        public Thickness OffsetMargin
        {
            get { return offsetMargin; }
            private set
            {
                if (offsetMargin != value)
                {
                    offsetMargin = value;
                    propertyChanged("OffsetMargin");
                }
            }
        }

        private Thickness offsetMargin = new Thickness(0);

        private void offsetChanged()
        {
            OffsetMargin = new Thickness(-HorizontalOffset, -VerticalOffset, 0, 0);
        }
        
        #endregion



        #region clipboard trapping

        private IDataObject previousDataObject = null;

        private byte[] pixelData;

        private int width, height;

        private Window mainWindow;

        public event EventHandler ClipboardUpdated;
        


        private void clipboardUpdate()
        {
            if (Clipboard.ContainsImage())
            {
                if (previousDataObject == null || !Clipboard.IsCurrent(previousDataObject))
                {
                    previousDataObject = Clipboard.GetDataObject();
                    var img = getClipboardImage();

                    Image = img;

                    var cu = ClipboardUpdated;
                    if (cu != null)
                        cu(this, new EventArgs());
                }
            }
            else
            {
                Image = null;
                previousDataObject = null;
            }
        }

        private BitmapSource getClipboardImage()
        {
            // get the clipboard image
            var img = Clipboard.GetImage();

            // get the current, real DPI setting (clipboard image always returns with a
            // value 96 dpis)
            PresentationSource source = PresentationSource.FromVisual(mainWindow);
            Debug.Assert(source != null);
            double dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            double dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;

            // creates a new bitmap source with the correct DPI values
            Debug.Assert(img.Format == PixelFormats.Bgra32);
            width = (int)img.Width;
            height = (int)img.Height;
            var stride = width * img.Format.BitsPerPixel / 8;
            pixelData = new byte[stride * height];
            img.CopyPixels(pixelData, stride, 0);
            BitmapSource bmpSource = BitmapSource.Create(
                (int)img.Width,
                (int)img.Height,
                dpiX,
                dpiY,
                img.Format,
                null,
                pixelData,
                stride);

            return bmpSource;
        }

        #endregion



        #region picker color grab

        /// <summary>
        /// The color currently under the picker.
        /// </summary>
        public Color PickerColor
        {
            get {
                if (!pickerColor.HasValue)
                    updatePickerSnapshot();
                return pickerColor.Value;
            }
            set
            {
                if (pickerColor != value)
                {
                    pickerColor = value;
                    numChanges++;
                    propertyChanged("PickerColor");
                }
            }
        }

        private Color? pickerColor = new Color();

        int numChanges = 0;



        public void ReloadPickerColor()
        {
            pickerColor = null;
            System.Diagnostics.Debug.WriteLine("Color: i am = {0}", GetHashCode());
            propertyChanged("PickerColor");
        }

        private void updatePickerSnapshot()
        {
            // find picker position in image coordinates (pixel)
            var toTransformMatrix = PresentationSource.FromVisual(mainWindow).CompositionTarget.TransformToDevice;
            var fromTransformMatrix = PresentationSource.FromVisual(mainWindow).CompositionTarget.TransformFromDevice;
            var scaledPickerPosition = toTransformMatrix.Transform(PickerPosition);
            var x = (int)(scaledPickerPosition.X / ZoomFactor);
            var y = (int)(scaledPickerPosition.Y / ZoomFactor);
            // find radius in image coordinates (pixel)
            var scaledPickerTopLeftCornerPosition = toTransformMatrix.Transform(PickerTopLeftCornerPosition);
            var pixelRadius = (int)((scaledPickerPosition.X - scaledPickerTopLeftCornerPosition.X) / ZoomFactor);
            // get pixel data from inside the circle
            int count = 0;
            int r = 0, g = 0, b = 0;
            for (var cy = y - pixelRadius; cy <= y + pixelRadius; cy++)
            {
                if (cy < 0) continue;
                if (cy >= height) continue;
                var delta = (int)Math.Sqrt(pixelRadius * pixelRadius + (y - cy) * (y - cy));
                for (var cx = x - delta; cx <= x + delta; cx++)
                {
                    if (cx < 0) continue;
                    if (cx >= width) continue;
                    var color = getPixelColor(cx, cy);
                    count++;
                    r += color.R;
                    g += color.G;
                    b += color.B;
                }
            }
            // compute the average
            var averageColor = new Color();
            if (count > 0)
            {
                averageColor.R = (byte)(r / count);
                averageColor.G = (byte)(g / count);
                averageColor.B = (byte)(b / count);
                averageColor.A = 255;
                System.Diagnostics.Debug.WriteLine("{0} pixels: raw r={1}, g={2}, b={3}, final r={4}, g={5}, b={6}",
                    count, r, g, b, r / count, g / count, b / count);
            }
            // save computed color
            PickerColor = averageColor;
            System.Diagnostics.Debug.WriteLine("Color recalculated to {0}", PickerColor);
        }

        private Color getPixelColor(int x, int y)
        {
            // get pixel color from image coordinates
            var stride = width * 4;
            int bytePosition = stride * y + x * 4;
            var color = new Color();
            color.B = pixelData[bytePosition];
            color.G = pixelData[bytePosition + 1];
            color.R = pixelData[bytePosition + 2];
            color.A = 255;
            return color;
        }

        #endregion



        #region color copy

        private class CopyToHarmonizerCommand : ICommand
        {
            private ClipboardManager cm;

            public CopyToHarmonizerCommand(ClipboardManager cm)
            {
                System.Diagnostics.Debug.WriteLine("Command: mine = {0}", cm.GetHashCode());
                this.cm = cm;
            }

            public bool CanExecute(object parameter)
            {
                // would be
                // return parameter != null && parameter is Arrows;
                // but wpf is stupid and doesn't re-run CanExecute when parameter changes, so
                // at first call, before binding, its value is null and this code would
                // always fail. great.
                return true;
            }

            public event EventHandler CanExecuteChanged { add { } remove { } }

            public void Execute(object parameter)
            {
                var arrows = parameter as Arrows;
                System.Diagnostics.Debug.WriteLine("Command: updating = {0}", cm.GetHashCode());
                arrows.Arrow0.Color = cm.PickerColor;
            }
        }

        public ICommand CopyToHarmonizer { get; private set; }

        private void initializeCopyToHarmonizer()
        {
            CopyToHarmonizer = new CopyToHarmonizerCommand(this);
        }

        #endregion



        #region low level clipboard management

        IntPtr installedHandle = IntPtr.Zero;

        private const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll")]
        private extern static bool AddClipboardFormatListener(IntPtr hWnd);

        [DllImport("user32.dll")]
        private extern static bool RemoveClipboardFormatListener(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)] 
        private extern static int SendMessage(IntPtr hWnd,int wMsg,IntPtr wParam,IntPtr lParam);

        private void hookEvents()
        {
            var hwndSource = PresentationSource.FromVisual(mainWindow) as HwndSource;
            if (hwndSource != null)
            {
                if (hwndSource.Handle == IntPtr.Zero)
                    mainWindow.SourceInitialized += mainWindow_SourceInitialized;
                else
                    hookEventsInner(hwndSource);
            }
        }

        void mainWindow_SourceInitialized(object sender, EventArgs e)
        {
            mainWindow.SourceInitialized -= mainWindow_SourceInitialized;
            var hwndSource = PresentationSource.FromVisual(mainWindow) as HwndSource;
            hookEventsInner(hwndSource);
        }

        private bool hookEventsInner(HwndSource hwndSource)
        {
            Debug.Assert(installedHandle == IntPtr.Zero);
            installedHandle = hwndSource.Handle;
            bool result = AddClipboardFormatListener(installedHandle);
            hwndSource.AddHook(hook);
            mainWindow.Closing += mainWindow_Closing;
            return result;
        }

        void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            unhookEvents();
            mainWindow.Closing -= mainWindow_Closing;
        }

        private bool unhookEvents()
        {
            if (installedHandle == IntPtr.Zero)
                return true;
            var rv = RemoveClipboardFormatListener(installedHandle);
            installedHandle = IntPtr.Zero;
            return rv;
        }

        private IntPtr hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_CLIPBOARDUPDATE)
                clipboardUpdate();
            return IntPtr.Zero;
        }

        #endregion



        #region notify property changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void propertyChanged(string propertyName)
        {
            var pc = PropertyChanged;
            if (pc != null)
                pc(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
