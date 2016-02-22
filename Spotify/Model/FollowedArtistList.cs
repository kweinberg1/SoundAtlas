using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify;
using Newtonsoft.Json;
namespace Spotify.Model
{
    public class ArtistSearchList : FollowedArtistList { }

    public class FollowedArtistList : IPaged
    {
        [JsonProperty("artists")]
        public ArtistItems ArtistItems { get; set; }
        public override String Next 
        { 
            get { return ArtistItems.Next; } 
            set { ArtistItems.Next = value; } 
        }

        #region Constructor
        public FollowedArtistList()
        {
            ArtistItems = new ArtistItems();
        }
        #endregion

        #region IPaged Implementation
        public override void Combine(IPaged pagedObject)
        {
            FollowedArtistList otherArtistList = (FollowedArtistList)pagedObject;

            this.ArtistItems.Combine(otherArtistList.ArtistItems);
        }
        #endregion
    }

    public class ArtistItems : IPaged
    {
        #region Properties
        [JsonProperty("items")]
        public List<Artist> Items;
        #endregion

        #region Constructor
        public ArtistItems()
        {
            Items = new List<Artist>();
        }
        #endregion

        #region IPaged Implementation
        public override void Combine(IPaged pagedObject)
        {
            ArtistItems otherArtistList = (ArtistItems)pagedObject;

            this.Items.AddRange(otherArtistList.Items);
        }
        #endregion
    }
}
