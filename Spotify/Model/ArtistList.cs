using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class ArtistList
    {
        [JsonProperty("artists")]
        public ArtistGroup ArtistGroup { get; set; }
    }

    public class ArtistGroup
    {
        #region Properties
        [JsonProperty("href")]
        public String Link;

        [JsonProperty("items")]
        public List<Artist> Items;

        [JsonProperty("artists")]
        public List<Artist> Artists;
        #endregion

        #region Constructor
        public ArtistGroup()
        {
            Link = null;
            Items = null;
            Artists = null;
        }
        #endregion
    }
}
