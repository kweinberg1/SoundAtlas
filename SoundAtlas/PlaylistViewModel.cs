using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Spotify;
using Spotify.Model;

namespace SoundAtlas
{
    public class PlaylistViewModel : INotifyPropertyChanged
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

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Event Handlers
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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

        public void AddArtistTracks(Artist artist)
        {
            IEnumerable<Track> addedTracks = AddArtistSongsToPlaylist(artist);

            PlaylistTracks = null;
            PlaylistTracks = _playlist.Tracks.Select(playlistTrack => playlistTrack.Track);
        }

        public IEnumerable<Track> AddArtistSongsToPlaylist(Artist artist)
        {
            const int numSongsPerArtist = 3;

            TrackList popularSongs = _client.GetArtistTopTracks(artist);

            IEnumerable<Track> songsToAdd = popularSongs.Tracks.Take(numSongsPerArtist);

            _client.AddTracksToPlaylist(_playlist, songsToAdd);

            //Update the playlist's track listings.
            PlaylistTrackList trackList = _client.GetPlaylistTracks(_playlist);
            _playlist.SetPlaylistTracks(trackList);

            return songsToAdd;
        }
    }
}
