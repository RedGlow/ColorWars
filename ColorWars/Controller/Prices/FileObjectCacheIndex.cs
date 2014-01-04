using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.Prices
{
    [Serializable]
    class FileObjectCacheIndex: Dictionary<string, FileObjectCacheEntry>
    {
        public FileObjectCacheIndex() {}

        public FileObjectCacheIndex(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
