namespace SoundAtlas2
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for PlaylistControl.xaml
    /// </summary>
    public partial class PlaylistControl : UserControl
    {
        #region Properties
        private PlaylistViewModel _viewModel;
        public event RoutedEventHandler PlaylistTrackSelectionChanged
        {
            add { AddHandler(PlaylistTrackSelectionChangedEvent, value); }
            remove { RemoveHandler(PlaylistTrackSelectionChangedEvent, value); }
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public PlaylistViewModel ViewModel => (PlaylistViewModel) DataContext;
        #endregion

        #region Events
        public readonly RoutedEvent PlaylistTrackSelectionChangedEvent = EventManager.RegisterRoutedEvent("PlaylistTrackSelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));
        #endregion

        #region Constructors
        public PlaylistControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Methods
        public void Initialize()
        {
            _viewModel = new PlaylistViewModel();
            this.DataContext = _viewModel;
        }
        #endregion

        #region Private Methods
        private void RaiseTrackSelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(PlaylistTrackSelectionChangedEvent, e);
            RaiseEvent(newEventArgs);
        }

        private void OnPlaylistListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseTrackSelectionChangedEvent(sender, e);
        }
        #endregion
    }
}
