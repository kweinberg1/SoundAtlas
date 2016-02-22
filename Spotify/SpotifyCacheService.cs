using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spotify.Model;
namespace Spotify
{
    /// <summary>
    /// Class to prevent constant calls to the Spotify API.
    /// </summary>
    public class SpotifyCacheService
    {
        private static FollowedArtistList _followedArtists;

        public static FollowedArtistList GetFollowedArtists()
        {
            _followedArtists = SpotifyClientService.Client.GetFollowedArtists();
            return _followedArtists;
        }

        public static bool IsArtistFollowed(Artist artist)
        {
            if (_followedArtists == null)
            {
                GetFollowedArtists();
            }

            return _followedArtists.ArtistItems.Items.Where(followedArtist => followedArtist.ID.Equals(artist.ID)).Any();
        }

        public static bool FollowArtist(Artist artist)
        {
            _followedArtists = null;
            return SpotifyClientService.Client.FollowArtist(artist);
        }

        public static bool UnfollowArtist(Artist artist)
        {
            _followedArtists = null;
            return SpotifyClientService.Client.UnfollowArtist(artist);
        }
    }
}
