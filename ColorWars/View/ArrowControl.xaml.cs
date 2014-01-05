using ColorWars.Controller.ColorHarmonizer;
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
    /// Interaction logic for ArrowControl.xaml
    /// </summary>
    public partial class ArrowControl : UserControl
    {
        public ArrowControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Data related to the start of the dragging during the dragging operation; null otherwise.
        /// </summary>
        private ArrowDraggingData arrowDraggingData = null;

        // TODO: make it readonly: http://stackoverflow.com/questions/1122595/how-do-you-create-a-read-only-dependency-property
        /// <summary>
        /// Whether the control is currently dragging the tip of the arrow.
        /// </summary>
        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDragging.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging", typeof(bool), typeof(ArrowControl), new PropertyMetadata(false));

        
        public void MyOnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //Console.WriteLine(string.Format("Left click on {0}, handled {1}.", this.Name, e.Handled));
            // super call
            base.OnPreviewMouseLeftButtonDown(e);

            // check if we clicked inside the color circle
            var clickPosition = e.GetPosition(colorCircle);
            var circleCenter = new Point(colorCircle.Width / 2, colorCircle.Height / 2);
            var circleRadius = colorCircle.Width / 2;
            var clickDistanceFromCenter = (clickPosition - circleCenter).Length;
            var nearEnough = clickDistanceFromCenter < circleRadius;

            if (!nearEnough)
                return;

            // save current values of rotation and scale
            var arrow = (Arrow)DataContext;
            var position = e.GetPosition(this);
            arrowDraggingData = new ArrowDraggingData(
                arrow.Angle, arrow.NormalizedLength,
                position,
                new Point(ActualWidth / 2, ActualHeight / 2));
            IsDragging = true;

            // tell the event has been handled
            e.Handled = true;
        }

        public void MyOnPreviewMouseMove(MouseEventArgs e)
        {
            // super call
            base.OnPreviewMouseMove(e);

            // check if the mouse button is still down
            if (e.LeftButton == MouseButtonState.Released)
                stopDrag();

            // check if we are in a dragging operation
            if (arrowDraggingData == null)
                return;

            // compute data for the new position
            var newArrowDraggingData = new ArrowDraggingData(
                0, 0, // actually not used
                e.GetPosition(this), new Point(ActualWidth / 2, ActualHeight / 2));

            // compute the new angle
            var deltaAngle = newArrowDraggingData.ClickAngle - arrowDraggingData.ClickAngle;
            var newAngle = arrowDraggingData.StartingAngle + deltaAngle;

            // compute the new normalized length
            var deltaMouseNormalizedLength = newArrowDraggingData.ClickNormalizedLength - arrowDraggingData.ClickNormalizedLength;
            var newNormalizedLength = arrowDraggingData.StartingNormalizedLength + deltaMouseNormalizedLength;

            // apply changes to model
            var arrow = (Arrow)DataContext;
            arrow.Angle = newAngle;
            arrow.NormalizedLength = newNormalizedLength;

            // we handled the event
            e.Handled = true;
        }

        public void MyOnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // super call
            base.OnPreviewMouseLeftButtonUp(e);

            // stop handling the drag
            stopDrag();
        }

        private void stopDrag()
        {
            arrowDraggingData = null;
            IsDragging = false;
        }
    }
}
