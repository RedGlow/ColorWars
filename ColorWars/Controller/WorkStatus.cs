using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ColorWars.Controller
{
    /// <summary>
    /// A singleton that contains the current work status.
    /// </summary>
    public class WorkStatus: DependencyObject
    {
        /// <summary>
        /// The current status.
        /// </summary>
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(WorkStatus), new PropertyMetadata(string.Empty));

        

        /// <summary>
        /// Create a new work status
        /// </summary>
        private WorkStatus()
        {
        }

        public override string ToString()
        {
            return Status;
        }

        /// <summary>
        /// The only WorkStatus instance.
        /// </summary>
        private static WorkStatus workStatus;

        /// <summary>
        /// Instance of the WorkStatus singleton.
        /// </summary>
        public static WorkStatus Instance
        {
            get
            {
                if (workStatus == null)
                    workStatus = new WorkStatus();
                return workStatus;
            }
        }

        /// <summary>
        /// Set a new work status.
        /// </summary>
        /// <param name="newStatus">The string of the new work status.</param>
        public void SetStatus(string newStatus) {
            Status = newStatus;
        }

        /// <summary>
        /// Remove an old work status.
        /// </summary>
        /// <param name="oldStatus">The work status to remove.</param>
        public void RemoveStatus(string oldStatus)
        {
            if (Status == oldStatus)
                Status = "";
        }
    }
}
