using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
namespace Spotify.Model
{
    [DebuggerDisplay("Artist {Name}")]
    public class Artist
    {
        #region Properties
        [JsonProperty("name")]
        public String Name
        {
            get;
            private set;
        }

        [JsonProperty("ID")]
        public String ID 
        { 
            get;
            set; 
        }

        [JsonProperty("followers")]
        public Dictionary<String, object> Followers
        {
            get;
            set;
        }

        [JsonProperty("genres")]
        public List<String> Genres
        {
            get;
            private set;
        }

        [JsonProperty("external_urls")]
        public Dictionary<String, String> ExternalURLs
        {
            get;
            private set;
        }

        [JsonProperty("popularity")]
        public int Popularity
        {
            get;
            private set;
        }
        #endregion

        public Artist()
        { 

        } 
    }
}
