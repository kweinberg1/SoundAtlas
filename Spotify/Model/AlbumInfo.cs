using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class AlbumInfo
    {
        #region Properties
        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("uri")]
        public String Uri { get; set; }

        [JsonProperty("external_urls")]
        public Dictionary<string, string> ExternalUrls { get; set; }

        [JsonProperty("album_type")]
        public AlbumType AlbumType { get; set; }  

        [JsonProperty("available_markets")]
        public List<String> AvailableMarkets { get; set; }

        [JsonProperty("href")]
        public String Link { get; set; }

        [JsonProperty("product")]
        public String Product { get; set; }

        [JsonProperty("images")]
        public List<Image> Images { get; set; }
        #endregion

        #region Constructors
        public AlbumInfo()
        {

        }
        #endregion
    }
}
