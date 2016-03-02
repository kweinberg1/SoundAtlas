using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotify.Model;
using Spotify;
using SoundAtlas2.Model;
namespace SoundAtlas2
{
    public class NavigationViewModel : ViewModelBase
    {
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


        #region Constructors
        public NavigationViewModel()
        {
            PlaylistList servicePlaylists = SpotifyClientService.Client.GetPlaylists(SpotifyClientService.User.Id);

            _playlists = servicePlaylists.Playlists;
        }
        #endregion
    }
}
