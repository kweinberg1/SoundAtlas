using System;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using NetworkUI;
using Utils;
using SoundAtlas2.Model;
using Spotify;
using Spotify.Model;
using ZoomAndPan;

namespace SoundAtlas2
{
    /// <summary>
    /// This is a Window that uses NetworkView to display a flow-chart.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.PlaylistView.Initialize(SpotifyClientService.User);
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public MainWindowViewModel ViewModel
        {
            get
            {
                return (MainWindowViewModel)DataContext;
            }
        }

        /// <summary>
        /// Event raised when the Window has loaded.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //
            // Display help text for the sample app.
            //
            HelpTextWindow helpTextWindow = new HelpTextWindow();
            helpTextWindow.Left = this.Left + this.Width + 5;
            helpTextWindow.Top = this.Top;
            helpTextWindow.Owner = this;
            helpTextWindow.Show();

            OverviewWindow overviewWindow = new OverviewWindow();
            overviewWindow.Left = this.Left;
            overviewWindow.Top = this.Top + this.Height + 5;
            overviewWindow.Owner = this;
            overviewWindow.DataContext = this.AtlasView.ViewModel; // Pass the view model onto the overview window.
            overviewWindow.Show();
        }

        #region Search Events
        private void OnSearchExecuted(object sender, RoutedEventArgs e)
        {
            IEnumerable<String> inputString = new List<String>() { (String)e.OriginalSource };
            //this.AtlasView.GenerateMap(inputString, (PlaylistViewModel)this.PlaylistView.DataContext);
        }
        #endregion

        #region Playlist Control Events
        /// <summary>
        /// Event raised when the user creates a new playlist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Event raised when the playist selection has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaylistSelectionChanged(object sender, RoutedEventArgs e)
        {
            PlaylistControl playlistControl = (PlaylistControl)sender;
            PlaylistViewModel playlistViewModel = (PlaylistViewModel)playlistControl.DataContext;

            IEnumerable<Artist> distinctArtists = playlistViewModel.PlaylistTracks.SelectMany(track => track.Artists).Distinct().Select(artist => artist);

            this.AtlasView.ViewModel.PlaylistViewModel = (PlaylistViewModel)this.PlaylistView.DataContext;

            this.AtlasView.ViewModel.CreateHierarchy(distinctArtists);
            this.AtlasView.ViewModel.GenerateNetwork();
            this.AtlasView.UpdateLayout();
            this.AtlasView.ViewModel.ArrangeNetwork();
        }

        private void OnAddPopularTracksClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel targetNodeViewModel = (NodeViewModel)(e.OriginalSource);
            ArtistViewModel targetViewModel = (ArtistViewModel)(targetNodeViewModel.ArtistViewModel);
            PlaylistViewModel playlistViewModel = (PlaylistViewModel)this.PlaylistView.DataContext;
            if (playlistViewModel != null)
            {
                targetViewModel.IsFlagged = true;
                playlistViewModel.AddArtistTracks(targetViewModel.Artist);
            }
        }

        private void OnAddArtistExecuted(object sender, RoutedEventArgs e)
        {
           
        }
        #endregion

        private void OnSearchPanelKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //For now, push the popup control.
                ArtistList searchResults = SpotifyClientService.Client.SearchArtists(this.SearchTextBox.Text);
                
                ((SearchControlViewModel)this.SearchControl.DataContext).SearchResults = searchResults.ArtistGroup.Items;
                this.SearchControl.IsOpen = true;
            }
        }

        private void OnSearchControlClosed(object sender, EventArgs e)
        {
            Artist selectedArtist = this.SearchControl.SelectedItem;

            List<Artist> artistList = new List<Artist>() { selectedArtist };
            this.AtlasView.ViewModel.AddArtistsToHierarchy(artistList);

            this.AtlasView.UpdateNetwork();
        }
    }
}
