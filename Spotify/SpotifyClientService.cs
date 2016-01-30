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
        public static String Country { get; private set; }

        static SpotifyClientService()
        {
            Client = new SpotifyClient();
            Country = "US"; //TODO:  Determine the user's current market.
        }

        public static bool Login()
        {
            User = Client.GetCurrentUser();

            return (User != null);
        }
    }
}
