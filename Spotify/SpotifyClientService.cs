using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spotify.Model;

namespace Spotify
{
    public class SpotifyClientService
    {
        public static SpotifyClient Client { get; private set; }
        public static User User { get; private set; }

        static SpotifyClientService()
        {
            Client = new SpotifyClient();
            User = Client.GetCurrentUser();
        }
    }
}
