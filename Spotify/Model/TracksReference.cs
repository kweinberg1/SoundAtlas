using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    /// <summary>
    /// This is a reference to a set of tracks in a playlist.
    /// </summary>
    public class TracksReference
    {
        [JsonProperty("href")]
        public String Url;

        [JsonProperty("total")]
        public int TrackCount;
    }
}
