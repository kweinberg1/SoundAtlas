using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    /// <summary>
    /// A playlist track is a song that has been specifically added to a playlist. 
    /// It contains data related to when the song has been added to the playlist, in addition to the normal Track data.
    /// </summary>
    public class PlaylistTrack
    {
        [JsonProperty("added_at")]
        public DateTime AddedAt { get; set; }

        //[JsonProperty("added_by")]
        //public String AddedBy { get; set; }

        [JsonProperty("track")]
        public Track Track { get; set; }

        [JsonProperty("is_local")]
        public Boolean IsLocal { get; set; }
    }

}
