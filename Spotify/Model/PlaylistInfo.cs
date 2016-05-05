namespace Spotify.Model
{
    using System;
    using Newtonsoft.Json;

    public class PlaylistInfo
    {
        #region Properties
        [JsonProperty("name")]
        public String Name
        {
            get; set; 
        }

        [JsonProperty("id")]
        public String ID
        {
            get; set;
        }

        [JsonProperty("owner")]
        public User UserInfo
        {
            get; set;
        }

        [JsonProperty("public")]
        public bool Public
        {
            get; set;
        }

        [JsonProperty("snapshot_id")]
        public String SnapshotID
        {
            get; set;
        }

        [JsonProperty("tracks")]
        public TracksReference TracksReference
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public PlaylistInfo() : this(String.Empty) { }

        /// <summary>
        /// Dummy constructor (for "new playlist" entry).
        /// </summary>
        /// <param name="name"></param>
        public PlaylistInfo(String name)
        {
            Name = name;
            ID = null;
            UserInfo = null;
        }

        public PlaylistInfo(PlaylistInfo info)
        {
            Name = info.Name;
            ID = info.ID;
            UserInfo = info.UserInfo;
        }
        #endregion
    }
}
