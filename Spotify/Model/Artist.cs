using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
namespace Spotify.Model
{
    [DataContract]
    [DebuggerDisplay("Artist {Name}")]
    public class Artist
    {
        #region Properties
        [DataMember]
        public String Name
        {
            get;
            private set;
        }

        [DataMember]
        public String ID;

        [DataMember(Name="followers")]
        private Dictionary<String, object> Followers;

        [DataMember]
        public List<String> Genres
        {
            get;
            private set;
        }

        //[DataMember]
        //private String SpotifyURL;

        [DataMember(Name="external_urls")]
        private Dictionary<String, String> ExternalURLs;

        [DataMember]
        private int Popularity;
        #endregion

        internal Artist() { } 

        internal Artist(String name, String id, Dictionary<String, object> followers, List<String> genres, int popularity, Dictionary<String, String> externalURLs)
        {
            Name = name;
            ID = id;
            Followers = followers;
            Genres = genres;
            Popularity = popularity;
            ExternalURLs = externalURLs;
        }

        public override bool Equals(object other)
        {
            Artist otherArtist = (Artist)other;
            return String.Equals(this.ID, otherArtist.ID);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
