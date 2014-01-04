using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColorWars.Model.Gw2SpidyApi
{
    /// <summary>
    /// A downloader for the GW2spidy results.
    /// </summary>
    public class Downloader
    {
        /// <summary>
        /// The queue of query tasks to perform
        /// </summary>
        private BlockingCollection<Task> downloadTasks = new BlockingCollection<Task>();

        /// <summary>
        /// The search URL, with {0} in place of the name of the dye.
        /// </summary>
        private const string searchUrl = "http://www.gw2spidy.com/api/v0.9/json/item-search/{0}/1";

        /// <summary>
        /// The scheduler used to run the download tasks.
        /// </summary>
        private TaskScheduler throttledTaskScheduler = new ThrottledTaskScheduler(new TimeSpan(0, 0, 1));

        /// <summary>
        /// Starts the download of a single dye, and returns a task with the data once completed.
        /// </summary>
        /// <param name="name">Name of the dye (must end with " Dye" and is case sensitive).</param>
        /// <returns>A task which, once completed, returns the data regarding a single dye.</returns>
        public Task<Result> Download(string name)
        {
            // check input
            if (!name.EndsWith(" Dye"))
                throw new InvalidOperationException("A dye name must end with ' Dye'.");
            return Task.Factory.StartNew(() =>
            {
                Debug.WriteLine("Starting the download of " + name);
                string downloadedJson;
                using (var wc = new WebClient())
                    downloadedJson = wc.DownloadString(string.Format(searchUrl, name));
                var results = JsonConvert.DeserializeObject<Results>(downloadedJson);
                // TODO: base dyes, dyes with no offers give no results
                return results.results[0];
            }, CancellationToken.None, TaskCreationOptions.LongRunning, throttledTaskScheduler);
        }
    }
}
