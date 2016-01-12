using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class TrackList
    {
        [JsonProperty("tracks")]
        public List<Track> Tracks { get; set; }
    }

    /// <summary>
    /// TODO: The only difference between these classes are the JsonProperty.  
    /// The Get Popular Tracks call has a different format than the track listing for a playlist.
    /// There's got to be a better way to drive the deserialization of the outputted JSON.
    /// </summary>
    public class PlaylistTrackList
    {
        [JsonProperty("items")]
        public List<PlaylistTrack> Tracks { get; set; }
    }
}
