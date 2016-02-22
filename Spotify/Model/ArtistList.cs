using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class ArtistList : IPaged
    {
        [JsonProperty("artists")]
        public ArtistGroup ArtistGroup { get; set; }

        #region Constructor
        public ArtistList()
        {
            ArtistGroup = new ArtistGroup();
        }
        #endregion

        #region IPaged Implementation
        public override void Combine(IPaged pagedObject)
        {
            ArtistList otherArtistList = (ArtistList)pagedObject;

            this.ArtistGroup.Combine(otherArtistList.ArtistGroup);
        }
        #endregion
    }

    public class ArtistGroup : IPaged
    {
        #region Properties
        [JsonProperty("artists")]
        public List<Artist> Artists;
        #endregion

        #region Constructor
        public ArtistGroup()
        {
            Artists = new List<Artist>();
        }
        #endregion

        #region IPaged Implementation
        public override void Combine(IPaged pagedObject)
        {
            ArtistGroup otherArtistList = (ArtistGroup)pagedObject;

            this.Artists.AddRange(otherArtistList.Artists);
        }
        #endregion
    }
}
