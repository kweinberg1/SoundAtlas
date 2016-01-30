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
            /*HelpTextWindow helpTextWindow = new HelpTextWindow();
            helpTextWindow.Left = this.Left + this.Width + 5;
            helpTextWindow.Top = this.Top;
            helpTextWindow.Owner = this;
            helpTextWindow.Show();
            */

            /*OverviewWindow overviewWindow = new OverviewWindow();
            overviewWindow.Left = this.Left;
            overviewWindow.Top = this.Top + this.Height + 5;
            overviewWindow.Owner = this;
            overviewWindow.DataContext = this.AtlasView.ViewModel; // Pass the view model onto the overview window.
            overviewWindow.Show();
            */

        }

        #region Search Events
        private const String DefaultSearchText = "Search";
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

        private void OnSearchPanelFocus(object sender, RoutedEventArgs e)
        {
            TextBox searchControlTextBox = (TextBox) sender;

            if (String.Equals(searchControlTextBox.Text, DefaultSearchText))
            {
                searchControlTextBox.Text = String.Empty;
            }
        }

        private void OnSearchPanelLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox searchPanelTextBox = (TextBox) sender;

            if (searchPanelTextBox.Text.Length == 0)
            {
                searchPanelTextBox.Text = DefaultSearchText;
            }
        }

        private void OnClearSearchTextButtonClick(object sender, RoutedEventArgs e)
        {
            this.SearchTextBox.Text = String.Empty;
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
            this.AtlasView.UpdateNetwork();
        }

        /// <summary>
        /// Handles button click for adding artist tracks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddTracksClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel targetNodeViewModel = (NodeViewModel)(e.OriginalSource);
            ArtistViewModel targetViewModel = (ArtistViewModel)(targetNodeViewModel.ArtistViewModel);
            PlaylistViewModel playlistViewModel = (PlaylistViewModel)this.PlaylistView.DataContext;
            if (playlistViewModel != null)
            {
                targetViewModel.IsFlagged = true;
                int trackCount = playlistViewModel.AddArtistTracks(targetViewModel.Artist);

                targetNodeViewModel.NumTracks += trackCount;
            }
        }

        private void OnAddArtistExecuted(object sender, RoutedEventArgs e)
        {
           //TODO: Placeholder.  This may not be required.
        }
        #endregion

        #region Login Methods
        LoginWindow openLoginWindow = null;

        private bool? ShowLoginWindow()
        {
            if (!SpotifyClientService.Client.HasAuthorizationAccess())
            {   
                openLoginWindow = new LoginWindow();
                openLoginWindow.ShowDialog();
                return openLoginWindow.DialogResult;
            }

            return true;
        }

        private void Login()
        {
            if (ShowLoginWindow() == true)
            {
                this.PlaylistView.Initialize();

                //TODO:  Support other music services.  This should be broken out into a separate UI action.
                LoginViewModel loginViewModel = (LoginViewModel)this.LoginControl.DataContext;
                loginViewModel.AccountName = SpotifyClientService.User.DisplayName;
                loginViewModel.MusicService = MusicService.Spotify;

                MainWindowViewModel mainWindowViewModel = (MainWindowViewModel)this.DataContext;
                mainWindowViewModel.IsLoggedIn = true;
            }
        }

        /// <summary>
        /// Event handler when the application has logged in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnAccountLogin(object sender, RoutedEventArgs e)
        {
            Login();
        }

        #endregion
    }
}
