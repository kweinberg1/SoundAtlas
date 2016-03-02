using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for NavigationControl.xaml
    /// </summary>
    public partial class NavigationControl : UserControl
    {
        private NavigationViewModel _viewModel;
        public NavigationViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public readonly RoutedEvent CreatePlaylistEvent = EventManager.RegisterRoutedEvent("CreatePlaylist", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));
        
        public event RoutedEventHandler CreatePlaylist
        {
            add { AddHandler(CreatePlaylistEvent, value); }
            remove { RemoveHandler(CreatePlaylistEvent, value); }
        }

        public readonly RoutedEvent PlaylistSelectionChangedEvent = EventManager.RegisterRoutedEvent("PlaylistSelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));
        public event RoutedEventHandler PlaylistSelectionChanged
        {
            add { AddHandler(PlaylistSelectionChangedEvent, value); }
            remove { RemoveHandler(PlaylistSelectionChangedEvent, value); }
        }

        public NavigationControl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            _viewModel = new NavigationViewModel();
            this.DataContext = _viewModel;
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

        private void OnPlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox playlistListBox  = (ListBox)sender;
            if (playlistListBox.SelectedItem == null)
            {
                return;
            }

            Playlist selectedPlaylist = (Playlist)playlistListBox.SelectedItem;

            if (selectedPlaylist.Tracks == null)
            {
                //Expand the playlist to include tracks.

                PlaylistTrackList playlistTracks = SpotifyClientService.Client.GetPlaylistTracks(selectedPlaylist);
                selectedPlaylist.SetPlaylistTracks(playlistTracks);
            }

            _viewModel.SelectedPlaylist = selectedPlaylist;

            //Reinitialize the Atlas control.
            RaiseSelectionChangedEvent(sender, e);
        }


        public void OnCreatePlaylist(Playlist playlist)
        {
            _viewModel.SelectedPlaylist = playlist;
            this.PlaylistListBox.SelectedItem = playlist;
        }

        public void SelectPlaylist(Playlist playlist)
        {
            _viewModel.SelectedPlaylist = playlist;
            this.PlaylistListBox.SelectedItem = playlist;
        }

    }
}
