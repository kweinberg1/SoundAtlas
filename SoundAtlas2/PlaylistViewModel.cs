using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Spotify;
using Spotify.Model;

using SoundAtlas2.Model;

namespace SoundAtlas2
{
    public class PlaylistViewModel : ViewModelBase
    {
        #region Members
        private Playlist _playlist;
        private PlaylistList _playlists;
        private SpotifyClient _client;
        #endregion 

        #region Properties
        public String PlaylistName
        {
            get { return _playlist.Name; }
        }

        private IEnumerable<Track> _playlistTracks;
        public IEnumerable<Track> PlaylistTracks
        {
            get { return _playlistTracks; }
            set
            {
                _playlistTracks = value;
                NotifyPropertyChanged();
            }
        }
        public Playlist Playlist
        {
            get { return _playlist; }
            set 
            { 
                _playlist = value;
                NotifyPropertyChanged();
            }
        }

        private List<Playlist> _displayPlaylists;
        public List<Playlist> Playlists
        {
            get { return _displayPlaylists; }
        }
        #endregion

        #region Constructors
        public PlaylistViewModel()
        {
            _playlist = null;
            _client = SpotifyClientService.Client;

            _playlists = _client.GetPlaylists(SpotifyClientService.User.Id);

            _displayPlaylists = _playlists.Playlists;
            _displayPlaylists.Insert(0, new Playlist("New Playlist"));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the track already exists in the playlist.
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        private bool ContainsTrack(Track targetTrack)
        {
            return _playlist.Tracks.Where(track => track.Track.ID == targetTrack.ID).Any();
        }

        public int AddArtistTracks(Artist artist)
        {
            IEnumerable<Track> addedTracks = AddArtistSongsToPlaylist(artist);

            PlaylistTracks = null;
            PlaylistTracks = _playlist.Tracks.Select(playlistTrack => playlistTrack.Track);
            return addedTracks.Count();
        }

        public IEnumerable<Track> AddArtistSongsToPlaylist(Artist artist)
        {
            const int numSongsPerArtist = 3;  //TODO: Expose this to the user.

            TrackList popularSongs = _client.GetArtistTopTracks(artist);

            List<Track> songsToAdd = new List<Track>(numSongsPerArtist); 

            foreach (Track popularTrack in popularSongs.Tracks)
            {
                if (!this.ContainsTrack(popularTrack))
                {
                    songsToAdd.Add(popularTrack);

                    if (songsToAdd.Count >= numSongsPerArtist)
                        break;
                }
            }

            if (!songsToAdd.Any())
            {
                AlbumList albumList = _client.GetArtistAlbums(artist);

                foreach (Album album in albumList.Items)
                {
                    AlbumTrackList albumTrackList = _client.GetAlbumTracks(album);
                    IEnumerable<Track> albumTracksByPopularity = albumTrackList.Tracks.OrderByDescending(track => track.Popularity);
                    foreach (Track albumTrack in albumTracksByPopularity)
                    {
                        if (!this.ContainsTrack(albumTrack))
                        {
                            songsToAdd.Add(albumTrack);

                            if (songsToAdd.Count >= numSongsPerArtist)
                                break;
                        }
                    }

                    if (songsToAdd.Count >= numSongsPerArtist)
                        break;
                }
            }

            _client.AddTracksToPlaylist(_playlist, songsToAdd);

            //Update the playlist's track listings.
            PlaylistTrackList trackList = _client.GetPlaylistTracks(_playlist);
            _playlist.SetPlaylistTracks(trackList);

            return songsToAdd;
        }

        /// <summary>
        /// Returns the number of tracks associated with the given artist.
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public int GetArtistTrackCount(Artist artist)
        {
            return _playlist.Tracks.SelectMany(track => track.Track.Artists).Where(trackArtist => trackArtist.ID == artist.ID).Count();
        }
        #endregion Methods
    }
}
