using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Model.Gw2api
{
    class Downloader
    {
#if DEBUG
        private static string fileUrl = @"..\..\colors.json";
#else
        private static string apiUrl = "https://api.guildwars2.com/v1/colors.json";
#endif

        /// <summary>
        /// The current status of this downloader.
        /// </summary>
        public Status Status { get; private set; }

        /// <summary>
        /// The result of the download status, available only when Status == Ready, otherwise null.
        /// </summary>
        public GW2ApiResult Result { get; private set; }

        /// <summary>
        /// Creates a new downloader
        /// </summary>
        public Downloader()
        {
            Status = Gw2api.Status.NotStarted;
        }

        /// <summary>
        /// Starts the download of the GW2 colors.
        /// </summary>
        public Task Run()
        {
            if (Status != Gw2api.Status.NotStarted)
                throw new InvalidOperationException("Download already started.");
            Status = Gw2api.Status.Working;
            var t = Task.Factory.StartNew(() =>
            {
                string downloadedJson;
#if DEBUG
                using (var sr = File.OpenText(fileUrl))
                    downloadedJson = sr.ReadToEnd();
#else
                // TODO: failure case
                using (var wc = new WebClient())
                    downloadedJson = wc.DownloadString(apiUrl);
#endif
                Result = JsonConvert.DeserializeObject<GW2ApiResult>(downloadedJson);
                Status = Gw2api.Status.Ready;
            });
            return t;
        }
    }
}
