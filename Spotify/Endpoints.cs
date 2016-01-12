using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Spotify.Utility;

namespace Spotify
{
    public class Endpoints
    {
        private static readonly String WebURL = "https://api.spotify.com";

        private static readonly String GetCurrentUserEndpoint = WebURL + "/v1/me";

        private static readonly String ArtistIDEndpoint = WebURL + "/v1/artists/{0}";
        private static readonly String ArtistEndpoint = WebURL + "/v1/search?q={0}&type=artist";
        private static readonly String GetArtistTopTracksEndpoint = WebURL + "/v1/artists/{0}/top-tracks?country=US";  //TODO:  Assumed United States (for localization).
        private static readonly String SearchGenreEndpoint = WebURL + "/v1/search?q=genre:\"{0}\"&type=artist";
        private static readonly String RelatedArtistEndpoint = WebURL + "/v1/artists/{0}/related-artists";

        private static readonly String CreatePlaylistEndpoint = WebURL + "/v1/users/{0}/playlists";
        private static readonly String GetPlaylistsEndpoint = WebURL + "/v1/users/{0}/playlists";
        private static readonly String GetPlaylistEndpoint = WebURL + "/v1/users/{0}/playlists/{1}";
        private static readonly String GetPlaylistTracksEndpoint = WebURL + "/v1/users/{0}/playlists/{1}/tracks";
        private static readonly String AddTracksToPlaylistEndpoint = WebURL + "/v1/users/{0}/playlists/{1}/tracks";


        private static readonly String AccountsURL = "https://accounts.spotify.com";
        private static readonly String AuthorizeEndpoint = AccountsURL + "/authorize/?client_id={0}&response_type={1}&redirect_uri={2}&state={3}&scope={4}&show_dialog={5}";
        private static readonly String AccessTokenEndpoint = AccountsURL + "/api/token";
        //TODO: Handle irregular characters (e.g. spaces); need to escape them in the REST query

        public static String GetCurrentUser()
        {
            return GetCurrentUserEndpoint;
        }

        public static String GetArtistByID(String artistId)
        {
            return String.Format(ArtistIDEndpoint, artistId);   
        }

        public static String GetArtist(String artistName)
        {
            return String.Format(ArtistEndpoint, artistName);
        }

        public static String GetArtistTopTracks(String artistId)
        {
            return String.Format(GetArtistTopTracksEndpoint, artistId);
        }

        public static String GetGenreArtists(String genreName)
        {
            return String.Format(SearchGenreEndpoint, genreName);
        }

        public static String GetRelatedArtists(String artistName)
        {
            return String.Format(RelatedArtistEndpoint, artistName);
        }

        public static String CreatePlaylist(String userId)
        {
            return String.Format(CreatePlaylistEndpoint, userId);
        }

        public static String GetPlaylist(String userId, String playlistId)
        {
            return String.Format(GetPlaylistEndpoint, userId, playlistId);
        }

        public static String GetPlaylists(String userId)
        {
            return String.Format(GetPlaylistsEndpoint, userId);
        }

        public static String GetPlaylistTracks(String userId, String playlistId)
        {
            return String.Format(GetPlaylistTracksEndpoint, userId, playlistId);
        }

        public static String AddTracksToPlaylist(String userId, String playlistId)
        {
            return String.Format(AddTracksToPlaylistEndpoint, userId, playlistId);
        }

        /// <summary>
        /// Example URL: https://accounts.spotify.com/authorize/?client_id=f78ea8ee8a054d56a6a90214a57ad830&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A8000
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public static String GetAuthorize(AuthorizationParameters authorizationParameters)
        {
            String scopeString = authorizationParameters.Scope.GetParameterNameAttribute<Spotify.Enums.Scope>(" ");

            return String.Format(AuthorizeEndpoint, authorizationParameters.ClientID, authorizationParameters.ResponseType,
                HttpUtility.UrlEncode(authorizationParameters.RedirectUri), authorizationParameters.State, scopeString, authorizationParameters.ShowDialog);
        }

        public static String GetAuthorizationToken()
        {
            return String.Format(AccessTokenEndpoint);
        }

    }
}
