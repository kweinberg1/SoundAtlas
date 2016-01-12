using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class Album
    {
        #region Properties
        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("uri")]
        public String Uri { get; set; }
        #endregion

        #region Constructors
        public Album()
        {

        }
        #endregion
    }
}
