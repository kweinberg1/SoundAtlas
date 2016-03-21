namespace SoundAtlas2
{
    using System;
    using System.IO;
    using System.Reflection;
    using Newtonsoft.Json;

    public class UserCache
    {
        #region Properties
        private readonly string _cacheLocation;
        private UserCacheData _cacheData;
        #endregion

        #region Constructors
        public UserCache(string accountName)
        {
            string applicationDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _cacheLocation = System.IO.Path.Combine(applicationDataDirectory, Assembly.GetExecutingAssembly().GetName().Name, string.Format("{0}.json", accountName));
            _cacheData = new UserCacheData();
        }
        #endregion

        #region Methods
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
        #endregion

        #region Serialization Methods
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
        #endregion
    }
}
