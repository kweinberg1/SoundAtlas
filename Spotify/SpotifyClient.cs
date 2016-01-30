using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Win32;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spotify.Enums;
using Spotify.Model;
using Spotify.Utility;

namespace Spotify
{
    public class SpotifyClient
    {
        #region Properties
        private String AuthorizationToken;
        private Token AccessToken;

        public delegate void LoginCallbackDelegate(String authorizeUrl);
        public LoginCallbackDelegate LoginCallback;
        public Action LoginFinishedCallback;
        #endregion

        #region Constructors
        public SpotifyClient()
        {
            AuthorizationToken = null;
        }
        #endregion 

        #region Web API Methods

        public User GetCurrentUser()
        {
            String getCurrentUserURL = Endpoints.GetCurrentUser();
            User user = GetRequest<User>(getCurrentUserURL, true);
            return user;
        }

        public ArtistList SearchArtists(String artistName)
        {
            try
            {
                String searchArtistsURL = Endpoints.SearchArtists(artistName);
                WebRequest artistRequest = HttpWebRequest.Create(searchArtistsURL);
                WebResponse artistResponse = artistRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(artistResponse.GetResponseStream()))
                {
                    String artistJsonResponse = streamReader.ReadToEnd();

                    ArtistList artistList = JsonConvert.DeserializeObject<ArtistList>(artistJsonResponse);
                    return artistList;
                }
            }
            catch(WebException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public ArtistList GetGenreArtists(String genreName)
        {
            try
            {
                String genreArtistsURL = Endpoints.GetGenreArtists(genreName);
                WebRequest genreArtistRequest = HttpWebRequest.Create(genreArtistsURL);
                WebResponse genreArtistResponse = genreArtistRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(genreArtistResponse.GetResponseStream()))
                {
                    String genreArtistJsonResponse = streamReader.ReadToEnd();

                    ArtistList genreArtists = JsonConvert.DeserializeObject<ArtistList>(genreArtistJsonResponse);
                    return genreArtists;
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public ArtistGroup GetRelatedArtists(String artistID)
        {
            try
            {
                String relatedArtistsURL = Endpoints.GetRelatedArtists(artistID);
                WebRequest relatedArtistRequest = HttpWebRequest.Create(relatedArtistsURL);
                WebResponse relatedArtistsURLArtistResponse = relatedArtistRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(relatedArtistsURLArtistResponse.GetResponseStream()))
                {
                    String relatedArtistJsonResponse = streamReader.ReadToEnd();

                    ArtistGroup relatedArtists = JsonConvert.DeserializeObject<ArtistGroup>(relatedArtistJsonResponse);
                    return relatedArtists;
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public AlbumList GetArtistAlbums(Artist artist)
        {
            AlbumList albumList = GetPagedRequest<AlbumList>(Endpoints.GetArtistAlbums(artist.ID, AlbumType.All), true);
            return albumList;
        }

        public AlbumTrackList GetAlbumTracks(Album album)
        {
            AlbumTrackList trackList = GetPagedRequest<AlbumTrackList>(Endpoints.GetAlbumTracks(album.ID), true);
            return trackList;
        }

        public TrackList GetArtistTopTracks(Artist artist)
        {
            String getArtistTopTracksURL = Endpoints.GetArtistTopTracks(artist.ID);
            TrackList trackList = GetRequest<TrackList>(getArtistTopTracksURL, false);
            return trackList;
        }

        public Playlist CreatePlaylist(String playlistName, String userId)
        {
            String createPlaylistURL = Endpoints.CreatePlaylist(userId);
            
            JObject body = new JObject
            {
                {"name", playlistName},
                {"public", "false"},  //Assume private playlists.
            };

            String requestBody = body.ToString();
            PlaylistInfo playlistInfo = PostRequest<PlaylistInfo>(createPlaylistURL, requestBody, true);
            Playlist newPlaylist = new Playlist(playlistInfo);
            return newPlaylist;
        }

        public PlaylistList GetPlaylists(String userId)
        {
            PlaylistInfoList playlistInfoList = GetPagedRequest<PlaylistInfoList>(Endpoints.GetPlaylists(userId), true);

            PlaylistList playlistList = new PlaylistList(playlistInfoList);
            return playlistList;
        }

        public PlaylistTrackList GetPlaylistTracks(PlaylistInfo playlist)
        {
            return GetRequest<PlaylistTrackList>(Endpoints.GetPlaylistTracks(playlist.UserInfo.Id, playlist.ID), true);
        }

        public void AddTracksToPlaylist(PlaylistInfo playlist, IEnumerable<Track> songsToAdd)
        {
            try
            {
                if (!HasAuthorizationAccess())
                {
                    if (!GetAuthorization())
                    {
                        Console.WriteLine("Unable to acquire authorization/access to the Spotify API.");
                        return;  //TODO:  Handle proper return codes.
                    }
                }

                using (WebClient wc = new WebClient())
                {
                    wc.Proxy = null;
                    wc.Headers.Add("Authorization", AccessToken.TokenType + " " + AccessToken.TokenCode);

                    NameValueCollection values = new NameValueCollection
                    {
                        {"client_id", Credentials.SoundAtlasClientID},
                        {"client_secret", Credentials.SoundAtlasClientSecret}
                    };

                    JObject body = new JObject
                    {
                        {"uris", JArray.FromObject(songsToAdd.Select(song => song.Uri))}
                    };
                    String bodyString = body.ToString();

                    byte[] data;

                    try
                    {
                        String getPlaylistTracksURL =  Endpoints.GetPlaylistTracks(playlist.UserInfo.Id, playlist.ID);
                        wc.UploadData(getPlaylistTracksURL, "POST", Encoding.UTF8.GetBytes(bodyString));
                    }
                    catch (WebException e)
                    {
                        using (StreamReader reader = new StreamReader(e.Response.GetResponseStream()))
                        {
                            data = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion 

        #region Accounts API Methods
        public bool HasAuthorizationAccess()
        {
            return AccessToken != null && !AccessToken.IsExpired();
        }

        public Task<bool> ListenForAuthorization()
        {
            Task<bool> authorizationTask = new Task<bool>(GetAuthorizationResponse);
            authorizationTask.Start();

            //TODO: Evaluate whether this can be removed entirely.
            System.Threading.Thread.Sleep(1000);

            return authorizationTask;
        }

        public String GetAuthorizationUrl()
        {
            AuthorizationParameters authorizationParameters = new AuthorizationParameters
            {
                ClientID = Credentials.SoundAtlasClientID,
                ResponseType = "code",
                RedirectUri = Credentials.SoundAtlasRedirectURL,
                State = "profile",
                Scope = Scope.PlaylistModifyPrivate | Scope.PlaylistModifyPublic | Scope.PlaylistReadPrivate | Scope.PlaylistReadCollaborative
            };

            String authorizeUrl = Endpoints.GetAuthorize(authorizationParameters);

            return authorizeUrl;
        }
        
        public bool GetAuthorization()
        {
            Task<bool> authorizationTask = new Task<bool>(GetAuthorizationResponse);
            authorizationTask.Start();

            //TODO: Evaluate whether this can be removed entirely.
            System.Threading.Thread.Sleep(1000);

            AuthorizationParameters authorizationParameters = new AuthorizationParameters
                {
                    ClientID = Credentials.SoundAtlasClientID,
                    ResponseType = "code",
                    RedirectUri = Credentials.SoundAtlasRedirectURL,
                    State = "profile",
                    Scope = Scope.PlaylistModifyPrivate | Scope.PlaylistModifyPublic | Scope.PlaylistReadPrivate | Scope.PlaylistReadCollaborative
                };

            String authorizeUrl = Endpoints.GetAuthorize(authorizationParameters);
            
            System.Diagnostics.Process.Start(authorizeUrl);
         
            authorizationTask.Wait();
            
            return authorizationTask.Result;
        }

        public bool GetAuthorizationResponse()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(Credentials.SoundAtlasRedirectURL);
            listener.Start();
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            String codeString = request.QueryString["code"];
            listener.Stop();

            AuthorizationToken = codeString;
            return GetAccessToken();
        }

        public bool GetAccessToken()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Proxy = null;
                    wc.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Credentials.SoundAtlasClientID + ":" + Credentials.SoundAtlasClientSecret)));

                    NameValueCollection values = new NameValueCollection
                    {
                        {"grant_type", "authorization_code"},
                        {"code", AuthorizationToken },
                        {"redirect_uri", Credentials.SoundAtlasRedirectURL },
                        {"client_id", Credentials.SoundAtlasClientID},
                        {"client_secret", Credentials.SoundAtlasClientSecret}
                    };

                    byte[] data;
                    try
                    {
                        String authorizationTokenURL = Endpoints.GetAuthorizationToken();
                        data = wc.UploadValues(authorizationTokenURL, "POST", values);
                    }
                    catch (WebException e)
                    {
                        using (StreamReader reader = new StreamReader(e.Response.GetResponseStream()))
                        {
                            data = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                        }
                    }

                    String responseString = Encoding.UTF8.GetString(data);
                    AccessToken = JsonConvert.DeserializeObject<Token>(responseString);
                    return String.IsNullOrEmpty(AccessToken.Error);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        #endregion

        #region Helper Functions
        public T PostRequest<T> (String url, String requestBody, bool useAuthorization)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = null;

                if (useAuthorization)
                    wc.Headers.Add("Authorization", AccessToken.TokenType + " " + AccessToken.TokenCode);

                NameValueCollection values = new NameValueCollection
                    {
                        {"client_id", Credentials.SoundAtlasClientID},
                        {"client_secret", Credentials.SoundAtlasClientSecret}
                    };

                byte[] data;

                try
                {
                    data = wc.UploadData(url, "POST", Encoding.UTF8.GetBytes(requestBody));
                }
                catch (WebException e)
                {
                    using (StreamReader reader = new StreamReader(e.Response.GetResponseStream()))
                    {
                        data = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                    }
                }
                
                String responseString = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject<T>(responseString);
            }
        }

        public T GetRequest<T>(String url, bool useAuthorization)
        {
            if (useAuthorization && !HasAuthorizationAccess())
            {
                if (!GetAuthorization())
                {
                    Console.WriteLine("Unable to acquire authorization/access to the Spotify API.");
                    return default(T);
                }
            }

            WebRequest webRequest = HttpWebRequest.Create(url);

            if (useAuthorization)
                webRequest.Headers.Add("Authorization", AccessToken.TokenType + " " + AccessToken.TokenCode);

            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";

            using (StreamReader streamReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                String response = streamReader.ReadToEnd();
                T responseObject = JsonConvert.DeserializeObject<T>(response);
                return responseObject;
            }
        }

        public T GetPagedRequest<T>(String url, bool useAuthorization) where T : IPaged, new()
        {
            String currentUrl = url;
            T masterList = new T();
            T returnedValue = null;
            do
            {
                returnedValue = GetRequest<T>(currentUrl, useAuthorization);

                //Combine these as part of the master list.
                masterList.Combine(returnedValue);

                IPaged pagingValue = (IPaged)returnedValue;
                currentUrl = pagingValue.Next;

            } while (returnedValue != null && returnedValue.Next != null);

            return masterList;
        }
        #endregion
    }
}
