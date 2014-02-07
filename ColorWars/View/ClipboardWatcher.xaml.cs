using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ColorWars.Controller;

namespace ColorWars.View
{
    /// <summary>
    /// Interaction logic for ClipboardWatcher.xaml
    /// </summary>
    public partial class ClipboardWatcher : UserControl
    {
        public ClipboardWatcher()
        {
            InitializeComponent();
        }



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ClipboardWatcher), new PropertyMetadata(null, new PropertyChangedCallback(commandChanged)));

        private static void commandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cw = d as ClipboardWatcher;
            cw.copyToHarmonizerButton.Command = cw.Command;
        }



        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ClipboardWatcher), new PropertyMetadata(null, new PropertyChangedCallback(commandPropertyChanged)));

        private static void commandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cw = d as ClipboardWatcher;
            cw.copyToHarmonizerButton.CommandParameter = cw.CommandParameter;
        }

        

        /// <summary>
        /// The clipboard manager controller object used to handle the image clipboard
        /// </summary>
        public ClipboardManager ClipboardManager
        {
            get { return (ClipboardManager)GetValue(ClipboardManagerProperty); }
            set { SetValue(ClipboardManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClipboardManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClipboardManagerProperty =
            DependencyProperty.Register("ClipboardManager", typeof(ClipboardManager), typeof(ClipboardWatcher), new PropertyMetadata(null));


        private bool isMouseOverImagePanel()
        {
            return clipboardImagePanel.IsMouseDirectlyOver ||
                clipboardImageGrid.IsMouseDirectlyOver ||
                clipboardImage.IsMouseDirectlyOver;
        }



        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            // check we are over the image area
            if (!isMouseOverImagePanel())
                return;

            var cm = DataContext as ClipboardManager;
            if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) != 0 ||
                (Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) != 0)
            {
                // ctrl + mouse wheel = zoom management
                if (e.Delta > 0)
                    cm.IncreaseZoom();
                else
                    cm.DecreaseZoom();
            }
            else
            {
                // mouse wheel = picker radius management
                if (e.Delta > 0)
                    cm.IncreasePickerDiameter();
                else
                    cm.DecreasePickerDiameter();
            }
            e.Handled = true;

            // super call
            base.OnPreviewMouseWheel(e);
        }



        public Cursor ImageCursor
        {
            get { return (Cursor)GetValue(ImageCursorProperty); }
            set { SetValue(ImageCursorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageCursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageCursorProperty =
            DependencyProperty.Register("ImageCursor", typeof(Cursor), typeof(ClipboardWatcher), new PropertyMetadata(Cursors.None));

        

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            // make the picker disappear
            (DataContext as ClipboardManager).HidePicker();
            base.OnMouseLeave(e);
        }

        private class DraggingInfo
        {
            public readonly Point ClickDownLocation;
            public readonly double StartingHorizontalOffset;
            public readonly double StartingVerticalOffset;
            public DraggingInfo(Point clickDownLocation,
                double startingHorizontalOffset,
                double startingVerticalOffset)
            {
                ClickDownLocation = clickDownLocation;
                StartingHorizontalOffset = startingHorizontalOffset;
                StartingVerticalOffset = startingVerticalOffset;
            }
        }

        private DraggingInfo draggingInfo;

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (!isMouseOverImagePanel())
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            draggingInfo = new DraggingInfo(
                Mouse.GetPosition(this),
                horizontalScrollbar.Value,
                verticalScrollbar.Value);
            ImageCursor = Cursors.Hand;
            (DataContext as ClipboardManager).HidePicker();
            e.Handled = true;

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (draggingInfo != null)
            {
                bool justClick = Mouse.GetPosition(this) == draggingInfo.ClickDownLocation;

                draggingInfo = null;
                ImageCursor = Cursors.None;
                updatePickerPosition(Mouse.GetPosition(clipboardImage));

                if (justClick)
                {
                    // this was a click, not a drag: copy color
                    (DataContext as ClipboardManager).ReloadPickerColor();
                }
            }

            base.OnPreviewMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (draggingInfo != null)
            {
                // dragging the image
                var newPosition = Mouse.GetPosition(this);
                var cm = DataContext as ClipboardManager;
                double h = cm.HorizontalOffset;
                double v = cm.VerticalOffset;
                cm.HorizontalOffset = draggingInfo.StartingHorizontalOffset + (draggingInfo.ClickDownLocation.X - newPosition.X);
                cm.VerticalOffset = draggingInfo.StartingVerticalOffset + (draggingInfo.ClickDownLocation.Y - newPosition.Y);
                System.Diagnostics.Debug.WriteLine("Horizontal: {0:0.00} => {1:0.00}", h, cm.HorizontalOffset);
                //clipboardImagePanel.ScrollToHorizontalOffset(
                //    draggingInfo.StartingHorizontalOffset + (draggingInfo.ClickDownLocation.X - newPosition.X));
                //clipboardImagePanel.ScrollToVerticalOffset(
                //    draggingInfo.StartingVerticalOffset + (draggingInfo.ClickDownLocation.Y - newPosition.Y));
            }
            else
            {
                // just moving around: update the picker position
                var position = e.GetPosition(clipboardImage);
                updatePickerPosition(position);
            }

            base.OnMouseMove(e);
        }

        private void updatePickerPosition(Point position)
        {
            var cm = DataContext as ClipboardManager;
            var scaledPosition = new Point(position.X * cm.ZoomFactor, position.Y * cm.ZoomFactor);
            cm.PickerPosition = scaledPosition;
        }
    }
}
