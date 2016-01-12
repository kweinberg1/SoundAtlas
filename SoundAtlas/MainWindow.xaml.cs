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
namespace SoundAtlas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            this.PlaylistView.Initialize(SpotifyClientService.User);
        }
        #endregion

        #region Event Handlers
        private void OnSearchExecuted(object sender, RoutedEventArgs e)
        {
            IEnumerable<String> inputString = new List<String>() { (String)e.OriginalSource };
            this.AtlasView.GenerateMap(inputString, (PlaylistViewModel)this.PlaylistView.DataContext);
        }

        private void OnCreatePlaylist(object sender, RoutedEventArgs e)
        {
            //Prompt users to name the playlist.
            CreatePlaylistDialog playlistDialog = new CreatePlaylistDialog();
            if (playlistDialog.ShowDialog() == true)
            {
                String playlistName = playlistDialog.PlaylistName;

                //Create the playlist in Spotify first.
                Playlist newPlaylist = SpotifyClientService.Client.CreatePlaylist(playlistName, SpotifyClientService.User.Id);

                this.PlaylistView.OnCreatePlaylist(newPlaylist);
            }
        }

        private void OnPlaylistSelectionChanged(object sender, RoutedEventArgs e)
        {
            PlaylistControl playlistControl = (PlaylistControl)sender;
            PlaylistViewModel playlistViewModel = (PlaylistViewModel)playlistControl.DataContext;

            IEnumerable<String> distinctArtists = playlistViewModel.PlaylistTracks.SelectMany(track => track.Artists).Distinct().Select(artist => artist.Name);
            this.AtlasView.GenerateMap(distinctArtists, (PlaylistViewModel)this.PlaylistView.DataContext);
        }

        #endregion
    }
}
