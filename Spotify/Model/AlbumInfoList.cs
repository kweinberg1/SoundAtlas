using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class AlbumInfoList : IPaged
    {
        #region Properties
        [JsonProperty("href")]
        public String Link;

        [JsonProperty("items")]
        public List<AlbumInfo> Items;

        #endregion

        #region Constructors
        public AlbumInfoList()
        {
            this.Items = new List<AlbumInfo>();
        }
        #endregion

        #region IPaged Implementation
        public override void Combine(IPaged pagedObject)
        {
            AlbumInfoList otherInfoList = (AlbumInfoList)pagedObject;
            this.Items.AddRange(otherInfoList.Items);
        }
        #endregion
    }
}
