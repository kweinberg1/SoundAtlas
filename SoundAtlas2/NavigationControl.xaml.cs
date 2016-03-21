namespace SoundAtlas2
{
    using System.Windows;
    using System.Windows.Controls;

    using Spotify;
    using Spotify.Model;


    /// <summary>
    /// Interaction logic for NavigationControl.xaml
    /// </summary>
    public partial class NavigationControl : UserControl
    {
        #region Properties
        private NavigationViewModel _viewModel;
        public NavigationViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }
        #endregion

        #region Events
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

        public readonly RoutedEvent RecommendEvent = EventManager.RegisterRoutedEvent("Recommend", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));
        public event RoutedEventHandler Recommend
        {
            add { AddHandler(RecommendEvent, value); }
            remove { RemoveHandler(RecommendEvent, value); }
        }
        #endregion

        #region Constructors
        public NavigationControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            _viewModel = new NavigationViewModel();
            this.DataContext = _viewModel;
        }
        
        public void SelectPlaylist(Playlist playlist)
        {
            _viewModel.SelectedPlaylist = playlist;
            this.PlaylistListBox.SelectedItem = playlist;
        }
        #endregion

        #region Event Handlers
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

        private void RaiseRecommendEvent(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(RecommendEvent, e);
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

        private void OnNewPlaylistButtonClick(object sender, RoutedEventArgs e) {

            RaiseCreatePlaylistEvent(sender, e);
        }

        private void OnRecommendButtonClick(object sender, RoutedEventArgs e)
        {
            RaiseRecommendEvent(sender, e);
        }
        #endregion
    }
}
