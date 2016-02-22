using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class Album : AlbumInfo
    {
        #region Properties
        [JsonProperty("release_date")]
        [JsonConverter(typeof(Spotify.Converters.DateTimeJsonConverter))]
        public DateTime ReleaseDate { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("artists")]
        public List<Artist> Artists { get; set; }
        #endregion

        #region Constructors
        public Album()
        {

        }
        #endregion
    }
}
