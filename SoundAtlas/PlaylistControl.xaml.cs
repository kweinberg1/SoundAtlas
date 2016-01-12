using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Spotify;
using Spotify.Model;

namespace SoundAtlas
{
    /// <summary>
    /// Interaction logic for PlaylistControl.xaml
    /// </summary>
    public partial class PlaylistControl : UserControl, INotifyPropertyChanged
    {
        #region Properties
        private PlaylistViewModel _viewModel;

        public event RoutedEventHandler CreatePlaylist
        {
            add { AddHandler(CreatePlaylistEvent, value); }
            remove { RemoveHandler(CreatePlaylistEvent, value); }
        }

        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly RoutedEvent CreatePlaylistEvent = EventManager.RegisterRoutedEvent("CreatePlaylist", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));
        public readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));
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

        public PlaylistControl()
        {
            InitializeComponent();
        }

        public void Initialize(User user)
        {
            _viewModel = new PlaylistViewModel();
            this.DataContext = _viewModel;
        }

        public void OnCreatePlaylist(Playlist playlist)
        {
            _viewModel.Playlist = playlist;
            _viewModel.Playlists.Add(playlist);
            PlaylistComboBox.SelectedItem = playlist;
        }

        private void OnAddPlaylist(object sender, RoutedEventArgs e)
        {
            RaiseCreatePlaylistEvent(sender, e);
        }

        private void RaiseCreatePlaylistEvent(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(CreatePlaylistEvent, e);
            RaiseEvent(newEventArgs);
        }

        private void RaiseSelectionChangedEvent(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(SelectionChangedEvent, e);
            RaiseEvent(newEventArgs);
        }

        private void OnPlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox playlistComboBox = (ComboBox)sender;
            if (playlistComboBox.SelectedIndex < 0) return;

            if (playlistComboBox.SelectedIndex == 0)
            {
                OnAddPlaylist(sender, new RoutedEventArgs());
            }
            else
            {
                Playlist selectedPlaylist = (Playlist)playlistComboBox.SelectedItem;

                if (selectedPlaylist.Tracks == null)
                {
                    //Expand the playlist to include tracks.

                    PlaylistTrackList playlistTracks = SpotifyClientService.Client.GetPlaylistTracks(selectedPlaylist);
                    selectedPlaylist.SetPlaylistTracks(playlistTracks);
                 }

                _viewModel.Playlist = selectedPlaylist;
                _viewModel.PlaylistTracks = selectedPlaylist.Tracks.Select(playlistTrack => playlistTrack.Track);

                //Reinitialize the Atlas control.
                RaiseSelectionChangedEvent(sender, e);
            }
        }
        

    }
}
