using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class AlbumList : IPaged
    {
        #region Properties
        [JsonProperty("href")]
        public String Link;

        [JsonProperty("items")]
        public List<Album> Items;

        #endregion

        #region Constructors
        public AlbumList()
        {
            this.Items = new List<Album>();
        }
        #endregion

        #region IPaged Implementation
        public override void Combine(IPaged pagedObject)
        {
            AlbumList otherInfoList = (AlbumList)pagedObject;
            this.Items.AddRange(otherInfoList.Items);
        }
        #endregion
    }
}
