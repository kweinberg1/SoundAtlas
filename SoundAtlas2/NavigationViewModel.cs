namespace SoundAtlas2
{
    using System.Collections.Generic;
    using Spotify.Model;
    using Spotify;
    using SoundAtlas2.Model;

    public class NavigationViewModel : ViewModelBase
    {
        #region Properties
        private List<Playlist> _playlists;
        public List<Playlist> Playlists
        {
            get { return _playlists; }
        }

        private Playlist _selectedPlaylist;
        public Playlist SelectedPlaylist
        {
            get 
            {
                return _selectedPlaylist; 
            }
            set 
            {
                _selectedPlaylist = value; 
                NotifyPropertyChanged(); 
            }
        }
        #endregion

        #region Constructors
        public NavigationViewModel()
        {
            PlaylistList servicePlaylists = SpotifyClientService.Client.GetPlaylists(SpotifyClientService.User.Id);
            
            _playlists = servicePlaylists.Playlists;
        }
        #endregion
    }
}
