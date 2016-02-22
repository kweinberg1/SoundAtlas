using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    /// <summary>
    /// A general track available within Spotify. 
    /// This object does not have to be part of a playlist.
    /// </summary>
    public class Track
    {
        #region Properties
        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("uri")]
        public String Uri { get; set; }

        [JsonProperty("artists")]
        public List<Artist> Artists { get; set; }

        [JsonProperty("album")]
        public AlbumInfo Album { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String ArtistDisplay
        {
            get
            {
                String[] artistNames = Artists.Select(artist => artist.Name).ToArray();
                return String.Join(",", artistNames);
            }
        }
        #endregion

        public Track()
        {

        }
    }
}
