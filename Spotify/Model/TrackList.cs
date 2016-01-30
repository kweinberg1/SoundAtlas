using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class TrackList : IPaged
    {
        [JsonProperty("tracks")]
        public List<Track> Tracks { get; set; }

        #region Constructors
        public TrackList()
        {
            Tracks = new List<Track>();
        }
        #endregion

        #region IPaged Implementation
        public override void Combine(IPaged pagedObject)
        {
            TrackList otherInfoList = (TrackList)pagedObject;
            this.Tracks.AddRange(otherInfoList.Tracks);
        }
        #endregion
    }

    /// <summary>
    /// TODO: The only difference between these classes are the JsonProperty.  
    /// The Get Popular Tracks call has a different format than the track listing for a playlist.
    /// There's got to be a better way to drive the deserialization of the outputted JSON.
    /// </summary>
    public class PlaylistTrackList : IPaged
    {
        #region Properties
        [JsonProperty("items")]
        public List<PlaylistTrack> Tracks { get; set; }
        #endregion

        #region Constructors
        public PlaylistTrackList()
        {
            Tracks = new List<PlaylistTrack>();
        }
        #endregion

        #region IPaged Implementation
        public override void Combine(IPaged pagedObject)
        {
            PlaylistTrackList otherInfoList = (PlaylistTrackList)pagedObject;
            this.Tracks.AddRange(otherInfoList.Tracks);
        }
        #endregion
    }

    /// <summary>
    /// TODO: The only difference between these classes are the JsonProperty.  
    /// The Get Popular Tracks call has a different format than the track listing for a playlist.
    /// There's got to be a better way to drive the deserialization of the outputted JSON.
    /// </summary>
    public class AlbumTrackList : IPaged
    {
        #region Properties
        [JsonProperty("items")]
        public List<Track> Tracks { get; set; }
        #endregion

        #region Constructors
        public AlbumTrackList()
        {
            Tracks = new List<Track>();
        }
        #endregion

        #region IPaged Implementation
        public override void Combine(IPaged pagedObject)
        {
            AlbumTrackList otherInfoList = (AlbumTrackList)pagedObject;
            this.Tracks.AddRange(otherInfoList.Tracks);
        }
        #endregion
    }
}
