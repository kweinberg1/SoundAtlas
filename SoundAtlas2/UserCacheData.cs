namespace SoundAtlas2
{
    using System.Collections.Generic;

    public class UserCacheData
    {
        public UserCacheData()
        {
            ViewedNewsItems = new HashSet<string>();
        }

        public HashSet<string> ViewedNewsItems;
    }
}
