using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoundAtlas2
{
    public class UserCacheData
    {
        public UserCacheData()
        {
            ViewedNewsItems = new HashSet<string>();
        }

        public HashSet<string> ViewedNewsItems;
    }

    public class UserCache
    {
        private string _cacheLocation;
        private UserCacheData _cacheData;

        public UserCache(string accountName)
        {
            string applicationDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _cacheLocation = System.IO.Path.Combine(applicationDataDirectory, Assembly.GetExecutingAssembly().GetName().Name, string.Format("{0}.json", accountName));
            _cacheData = new UserCacheData();
        }

        public void LogViewedNewsItem(string key)
        {
            if (!_cacheData.ViewedNewsItems.Contains(key))
            {
                _cacheData.ViewedNewsItems.Add(key);
                Save();
            }
        }

        public bool SeenNewsItem(string key)
        {
            return _cacheData.ViewedNewsItems.Contains(key);
        }

        public static UserCache Load(string userName)
        {
            UserCache newCache = new UserCache(userName);

            if (File.Exists(newCache._cacheLocation))
            {
                using (StreamReader reader = new StreamReader(newCache._cacheLocation))
                {
                    newCache._cacheData = JsonConvert.DeserializeObject<UserCacheData>(reader.ReadToEnd());
                }
            }

            if (newCache._cacheData == null)
            {
                newCache._cacheData = new UserCacheData();
            }

            return newCache;
        }

        public bool Save()
        {
            try
            {
                string directoryName = Path.GetDirectoryName(_cacheLocation);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                using (StreamWriter writer = new StreamWriter(_cacheLocation))
                {
                    string json = JsonConvert.SerializeObject(this._cacheData, Formatting.Indented);
                    writer.WriteLine(json);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
