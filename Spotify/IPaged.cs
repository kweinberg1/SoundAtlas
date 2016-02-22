using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify
{
    public abstract class IPaged
    {
        [JsonProperty("href")]
        public String Uri { get; set; }

        [JsonProperty("previous")]
        public String Previous { get; set; }

        [JsonProperty("next")]
        public virtual String Next { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        public abstract void Combine(IPaged pagedObject);
    }
}
