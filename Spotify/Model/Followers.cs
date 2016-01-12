﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify.Model
{
    public class Followers
    {
        [JsonProperty("href")]
        public String Href { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
