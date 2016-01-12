using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spotify.Model;
using Newtonsoft.Json;

namespace Spotify
{
    internal static class SpotifyClientTestMethods
    {
        #region Test Serialization
        private static void TestSerializeArtist(this SpotifyClient client)
        {
            List<String> genres = new List<String> { "chillwave", "grave wave", "metropopolis" };
            Dictionary<String, String> externalURLs = new Dictionary<String, String>();
            externalURLs.Add("Spotify", "http://...");
            Dictionary<String, object> followers = new Dictionary<String, object>();
            followers.Add("follower_count", 131331);
            Artist grimes = new Artist("Grimes", "053q0ukIDRgzwTr4vNSwab", followers, genres, 75, externalURLs);
            string serialized = JsonConvert.SerializeObject(grimes);

            using (StreamWriter writer = new StreamWriter("testdata.txt"))
            {
                writer.WriteLine(serialized);
            }
        }
        #endregion
    }
}
