using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ColorWars.Controller.Prices
{
    /// <summary>
    /// An ObjectCache based on a file.
    /// </summary>
    class FileObjectCache: ObjectCache
    {
        /// <summary>
        /// Creates a new FileObjectCache.
        /// </summary>
        /// <param name="dirname">Name of the directory where to store the cache entries.</param>
        public FileObjectCache(string dirname)
        {
            // get cache directory
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            cacheDirectoryPath = Path.Combine(appData, dirname);
            // create it if necessary
            if (!Directory.Exists(cacheDirectoryPath))
                Directory.CreateDirectory(cacheDirectoryPath);
            // create or load index file
            indexFilePath = Path.Combine(cacheDirectoryPath, "index.dat");
            if (File.Exists(indexFilePath))
                using (var fs = new FileStream(indexFilePath, FileMode.Open))
                    indexData = (FileObjectCacheIndex)binaryFormatter.Deserialize(fs);
            else
                indexData = new FileObjectCacheIndex();
            // save name
            name = dirname;
        }

        /// <summary>
        /// The path where the cache is stored.
        /// </summary>
        private string cacheDirectoryPath;

        /// <summary>
        /// Path to the serialized version of the index.
        /// </summary>
        private string indexFilePath;

        /// <summary>
        /// A binary formatter used by this cache.
        /// </summary>
        private BinaryFormatter binaryFormatter = new BinaryFormatter();

        /// <summary>
        /// Index data.
        /// </summary>
        private FileObjectCacheIndex indexData;
        
        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            return addOrSetEntry(key, value, absoluteExpiration, false);
        }

        private object addOrSetEntry(string key, object value, DateTimeOffset absoluteExpiration, bool overrideEntry)
        {
            // remove entry if too old
            var hashCode = key.GetHashCode();
            checkIfRemove(key);
            // return false if the entry already exists
            var entryFile = Path.Combine(cacheDirectoryPath, hashCode.ToString());
            if (File.Exists(entryFile))
            {
                if (overrideEntry)
                    File.Delete(entryFile);
                else
                    return getEntry(hashCode);
            }
            // serialize to file
            using (var fs = new FileStream(entryFile, FileMode.Create, FileAccess.Write, FileShare.None))
                binaryFormatter.Serialize(fs, value);
            // update index
            indexData[key] = new FileObjectCacheEntry(absoluteExpiration);
            // save index
            saveIndexData();
            // all done
            return null;
        }

        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            // add operations with CacheItemPolicy are not supported
            throw new NotImplementedException();
        }

        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            // add operations with CacheItemPolicy are not supported
            throw new NotImplementedException();
        }

        public override long GetCount(string regionName = null)
        {
            return indexData.Count;
        }

        public override object Remove(string key, string regionName = null)
        {
            var hashCode = key.GetHashCode();
            if (indexData.ContainsKey(key))
            {
                var removedEntry = getEntry(hashCode);
                indexData.Remove(key);
                File.Delete(Path.Combine(cacheDirectoryPath, hashCode.ToString()));
                return removedEntry;
            }
            else
                return null;
        }

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            var d = new Dictionary<string, object>();
            foreach (var key in keys)
            {
                checkIfRemove(key);
                d[key] = getEntry(key.GetHashCode());
            }
            return d;
        }

        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            addOrSetEntry(key, value, absoluteExpiration, true);
        }

        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            // add operations with CacheItemPolicy are not supported
            throw new NotImplementedException();
        }

        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            // add operations with CacheItemPolicy are not supported
            throw new NotImplementedException();
        }

        public override object Get(string key, string regionName = null)
        {
            checkIfRemove(key);
            var hashCode = key.GetHashCode();
            var path = Path.Combine(cacheDirectoryPath, hashCode.ToString());
            if (File.Exists(path))
                return getEntry(hashCode);
            else
                return null;
        }

        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            checkIfRemove(key);
            var o = Get(key, regionName);
            if (o == null)
                return null;
            else
                return new CacheItem(key, o);
        }

        public override bool Contains(string key, string regionName = null)
        {
            checkIfRemove(key);
            return indexData.ContainsKey(key);
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var key in indexData.Keys)
            {
                checkIfRemove(key);
                yield return new KeyValuePair<string, object>(key, getEntry(key.GetHashCode()));
            }
        }

        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override object this[string key]
        {
            get
            {
                checkIfRemove(key);
                return getEntry(key.GetHashCode());
            }
            set
            {
                Set(key, value, DateTimeOffset.MaxValue);
            }
        }

        private string name;

        public override string Name
        {
            get { return name; }
        }

        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get
            {
                return DefaultCacheCapabilities.AbsoluteExpirations | DefaultCacheCapabilities.InMemoryProvider;
            }
        }

        /// <summary>
        /// Save the indexData to file.
        /// </summary>
        private void saveIndexData()
        {
            using (var fs = new FileStream(indexFilePath, FileMode.Create))
                binaryFormatter.Serialize(fs, indexData);
        }

        /// <summary>
        /// Removed an entry from filesystem and index if expired. Doesn't save the new index.
        /// </summary>
        /// <param name="hashCode">Hashcode of the entry to check.</param>
        private void checkIfRemove(string key)
        {
            var hashCode = key.GetHashCode();
            var filename = Path.Combine(cacheDirectoryPath, hashCode.ToString());
            // check if we have the entry, and be sure the file is deleted if we don't have it
            if (!indexData.ContainsKey(key))
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                return;
            }
            // erase the entry if it expired;
            if(DateTimeOffset.Now > indexData[key].Expiration)
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                indexData.Remove(key);
            }
        }

        /// <summary>
        /// Get the content of an entry.
        /// </summary>
        /// <param name="hashCode">The hash code of the entry.</param>
        /// <returns>The object in the cache.</returns>
        private object getEntry(int hashCode)
        {
            using (var fs = new FileStream(Path.Combine(cacheDirectoryPath, hashCode.ToString()), FileMode.Open))
                return binaryFormatter.Deserialize(fs);
        }
    }
}
