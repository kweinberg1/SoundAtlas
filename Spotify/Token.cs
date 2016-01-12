using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Spotify
{
    public class Token
    {
        #region Properties
        [JsonProperty("access_token")]
        public String TokenCode { get; set; }

        [JsonProperty("token_type")]
        public String TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public String RefreshToken;

        [JsonProperty("error")]
        public String Error { get; set; }

        [JsonProperty("error_description")]
        public String ErrorDescription { get; set; }

        private DateTime TimeCreated;

        #endregion

        public Token()
        {
            TokenCode = null;
            TokenType = null;
            ExpiresIn = 0;
            RefreshToken = null;
            TimeCreated = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            return TimeCreated.Add(TimeSpan.FromSeconds(ExpiresIn)) <= DateTime.UtcNow;
        }
    }
}
