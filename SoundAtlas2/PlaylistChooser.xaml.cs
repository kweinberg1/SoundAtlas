namespace SoundAtlas2
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using SoundAtlas2.Model;
    using Spotify.Model;
    /// <summary>
    /// Interaction logic for PlaylistChooser.xaml
    /// </summary>
    public partial class PlaylistChooser : Popup
    {
        #region Properties

        public Playlist SelectedPlaylist
        {
            get;
            private set;
        }
        #endregion 

        #region Constructors
        public PlaylistChooser()
        {
            InitializeComponent();

            this.DataContext = new PlaylistChooserViewModel();
        }
        #endregion

        #region Event Handlers
        private void OnPlaylistSelectionChanged(object sender, SelectionChangedEventArgs e) {

            ListBox playlistListBox = (ListBox)sender;
            if (playlistListBox.SelectedItem == null)
            {
                return;
            }

            SelectedPlaylist = (Playlist)playlistListBox.SelectedItem;
            this.IsOpen = false;
        }
        #endregion
    }
}
