using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.Prices
{
    [Serializable]
    class FileObjectCacheEntry
    {
        public DateTimeOffset Expiration { get; set; }
        public FileObjectCacheEntry(DateTimeOffset expiration)
        {
            Expiration = expiration;
        }
    }
}
