namespace Spotify.Model
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    
    public class Playlist : PlaylistInfo 
    {
        #region Properties
        [JsonProperty("tracks")]
        public List<PlaylistTrack> Tracks
        {
            get;
            private set;
        }

        [JsonIgnore]
        public bool IsEditable => (this.UserInfo.Id.Equals(SpotifyClientService.User.Id));
        #endregion

        #region Constructors
        /// <summary>
        /// Dummy constructor (for "new playlist" entry).
        /// </summary>
        /// <param name="name"></param>
        [JsonConstructor()]
        public Playlist(String name) : base(name)
        {
        }

        /// <summary>
        /// "Copy" constructor to allow for the creation of full playlists.
        /// </summary>
        /// <param name="info"></param>
        public Playlist(PlaylistInfo info)
            : base(info)
        {
            Tracks = null;
        }

        /// <summary>
        /// Sets the full info of the playlist's tracks.
        /// </summary>
        /// <param name="trackList"></param>
        public void SetPlaylistTracks(PlaylistTrackList trackList)
        {
            Tracks = new List<PlaylistTrack>();
            Tracks.AddRange(trackList.Tracks);
        }
        #endregion
    }
}
