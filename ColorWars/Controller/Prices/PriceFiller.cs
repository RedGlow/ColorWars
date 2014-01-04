using ColorWars.Controller.Colors;
using ColorWars.Model.Gw2SpidyApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.Prices
{
    class PriceFiller
    {
        /// <summary>
        /// The downloader used to get all the prices.
        /// </summary>
        static Downloader downloader = null;

        /// <summary>
        /// The cache of gw2spidy cache objects downloaded.
        /// </summary>
        private static ObjectCache priceCache = new FileObjectCache("ColorWars-Gw2spidy");

        /// <summary>
        /// Duration of a cache entry.
        /// </summary>
        private static TimeSpan cacheDuration = new TimeSpan(0, 5, 0);

        /// <summary>
        /// Create a new PriceFiller.
        /// </summary>
        public PriceFiller(Dye dye)
        {
            // create a download if necessary
            if (downloader == null)
                downloader = new Downloader();
            // get the object from cache if available
            var cachedValue = priceCache.Get(dye.Name);
            if (cachedValue != null)
            {
                dye.Price = (Price)cachedValue;
                return;
            }
            // download the price data regarding the dye
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            downloader.Download(dye.Name + " Dye").ContinueWith((task) =>
            {
                dye.Price = new Price(task.Result);
                priceCache.Set(dye.Name, dye.Price, new DateTimeOffset(DateTime.Now + cacheDuration));
            }, scheduler);
        }
    }
}
