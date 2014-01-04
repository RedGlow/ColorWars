using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColorWars.Model.Gw2SpidyApi
{
    /// <summary>
    /// A TaskScheduler that allows the execution of tasks only in a serial way, and throttling the speed.
    /// </summary>
    class ThrottledTaskScheduler: TaskScheduler
    {
        /// <summary>
        /// Minimum distance between two task executions
        /// </summary>
        public readonly TimeSpan MinimumTimeSpan;

        /// <summary>
        /// Creates a new ThrottledTaskScheduler.
        /// </summary>
        /// <param name="minimumTimeSpan">The minumum time between two executions.</param>
        public ThrottledTaskScheduler(TimeSpan minimumTimeSpan)
        {
            MinimumTimeSpan = minimumTimeSpan;

            // start the queue processor
            Task.Factory.StartNew(queueDownloader, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// All the tasks waiting to be executed.
        /// </summary>
        private BlockingCollection<Task> tasks = new BlockingCollection<Task>();

        protected override void QueueTask(Task task)
        {
            tasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // disable inline execution
            return false;
        }

        protected override bool TryDequeue(Task task)
        {
            // disable dequeue
            return false;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return tasks.AsEnumerable();
        }

        /// <summary>
        /// Thread continuously making queries
        /// </summary>
        private void queueDownloader()
        {
            var lastRunTime = DateTime.MinValue;
            for (; ; )
            {
                // get a new task
                var task = tasks.Take();
                // sleep a while before executing it, if it's too early
                var elapsedTime = lastRunTime == DateTime.MinValue ? MinimumTimeSpan : DateTime.Now - lastRunTime;
                if (elapsedTime < MinimumTimeSpan)
                {
                    Debug.WriteLine("Got an early call, waiting a bit.");
                    System.Threading.Thread.Sleep(MinimumTimeSpan - elapsedTime);
                }
                // run the task and wait for its completion
                Debug.WriteLine("Starting the download.");
                lastRunTime = DateTime.Now;
                TryExecuteTask(task);
                task.Wait();
            }
        }
    }
}
