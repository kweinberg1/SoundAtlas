using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class PlaylistList
    {
        [JsonProperty("href")]
        public String Uri { get; set; }
 
        [JsonProperty("items")]
        public List<Playlist> Playlists { get; set; }

        public PlaylistList(PlaylistInfoList infoList)
        {
            Uri = infoList.Uri;
            Playlists = infoList.Playlists.Select(playlistInfo => new Playlist(playlistInfo)).ToList();
        }
    }

    public class PlaylistInfoList : IPaged
    {
        #region Properties
        [JsonProperty("items")]
        public List<PlaylistInfo> Playlists { get; set; }
        #endregion

        #region Constructors
        public PlaylistInfoList()
        {
            Playlists = new List<PlaylistInfo>();
        }
        #endregion

        #region IPaged Implementation
        public override void Combine(IPaged pagedObject)
        {
            PlaylistInfoList otherInfoList = (PlaylistInfoList)pagedObject;
            this.Playlists.AddRange(otherInfoList.Playlists);
        }
        #endregion
    }
}
