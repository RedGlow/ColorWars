using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ColorWars.Controller
{
    /// <summary>
    /// Controller for a pop-up window search text.
    /// </summary>
    public class TextSearcher: DependencyObject
    {
        /// <summary>
        /// The string currently searched in the list of dyes.
        /// </summary>
        public string CurrentlySearchedString
        {
            get { return (string)GetValue(CurrentlySearchedStringProperty); }
            set { SetValue(CurrentlySearchedStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentlySearchedString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentlySearchedStringProperty =
            DependencyProperty.Register("CurrentlySearchedString", typeof(string), typeof(TextSearcher),
            new PropertyMetadata(string.Empty, currentlySearchedStringChanged));

        private static void currentlySearchedStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textSearcher = (TextSearcher)d;
            textSearcher.updateTimer();
            textSearcher.Searching = textSearcher.CurrentlySearchedString != string.Empty;
        }


        /// <summary>
        /// Whether some string is being searched.
        /// </summary>
        public bool Searching
        {
            get { return (bool)GetValue(SearchingProperty); }
            set { SetValue(SearchingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Searching.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchingProperty =
            DependencyProperty.Register("Searching", typeof(bool), typeof(TextSearcher), new PropertyMetadata(false));

        

        private void updateTimer()
        {
            // reset the timer if something significant was written
 	        if(CurrentlySearchedString != string.Empty)
            {
                timer.Stop();
                timer.Start();
            }
        }
        

        /// <summary>
        /// Timer used to reset the text
        /// </summary>
        private DispatcherTimer timer;


        public TextSearcher() {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
        }


        void timer_Tick(object sender, EventArgs e)
        {
            // too long a time has passed since last input: empty the string.
            CurrentlySearchedString = string.Empty;
        }
    }
}
