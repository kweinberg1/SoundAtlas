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

namespace SoundAtlas2
{
    /// <summary>
    /// Interaction logic for PlaylistControl.xaml
    /// </summary>
    public partial class PlaylistControl : UserControl
    {
        #region Properties
        private PlaylistViewModel _viewModel;

        public event RoutedEventHandler CreatePlaylist
        {
            add { AddHandler(CreatePlaylistEvent, value); }
            remove { RemoveHandler(CreatePlaylistEvent, value); }
        }

        public event RoutedEventHandler PlaylistSelectionChanged
        {
            add { AddHandler(PlaylistSelectionChangedEvent, value); }
            remove { RemoveHandler(PlaylistSelectionChangedEvent, value); }
        }

        public event RoutedEventHandler PlaylistTrackSelectionChanged
        {
            add { AddHandler(PlaylistTrackSelectionChangedEvent, value); }
            remove { RemoveHandler(PlaylistTrackSelectionChangedEvent, value); }
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public PlaylistViewModel ViewModel
        {
            get
            {
                return (PlaylistViewModel)DataContext;
            }
        }
        #endregion

        #region Events
        public readonly RoutedEvent CreatePlaylistEvent = EventManager.RegisterRoutedEvent("CreatePlaylist", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));
        public readonly RoutedEvent PlaylistSelectionChangedEvent = EventManager.RegisterRoutedEvent("PlaylistSelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));
        public readonly RoutedEvent PlaylistTrackSelectionChangedEvent = EventManager.RegisterRoutedEvent("PlaylistTrackSelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));
        #endregion


        public PlaylistControl()
        {
            InitializeComponent();
        }

        public void Initialize()
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

        public void SelectPlaylist(Playlist playlist)
        {
            _viewModel.Playlist = playlist;
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
            RoutedEventArgs newEventArgs = new RoutedEventArgs(PlaylistSelectionChangedEvent, e);
            RaiseEvent(newEventArgs);
        }

        private void RaiseTrackSelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(PlaylistTrackSelectionChangedEvent, e);
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

        private void OnPlaylistListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseTrackSelectionChangedEvent(sender, e);
        }
    }
}
