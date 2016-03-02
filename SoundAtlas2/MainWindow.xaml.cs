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
        #region Fields
        private LoginWindow _openLoginWindow;
        private UserCache _userCache;
        #endregion

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

        void AddTracks(NodeViewModel targetNodeViewModel)
        {
            ArtistViewModel targetViewModel = (ArtistViewModel)(targetNodeViewModel.ArtistViewModel);
            PlaylistViewModel playlistViewModel = (PlaylistViewModel)this.PlaylistView.DataContext;
            if (playlistViewModel != null)
            {
                targetViewModel.IsFlagged = true;
                int trackCount = playlistViewModel.AddArtistTracks(targetViewModel.Artist);

                targetNodeViewModel.NumTracks += trackCount;
            }
        }

        #region Search Events
        private const String DefaultSearchText = "Search";
        private void OnSearchPanelKeyDown(object sender, KeyEventArgs e)
        {
            TextBox searchPanelTextBox = (TextBox)sender;
            if (searchPanelTextBox.Text.Length > 0)
            {
                this.ClearSearchButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.ClearSearchButton.Visibility = System.Windows.Visibility.Hidden;
            }

            if (e.Key == Key.Enter)
            {
                //For now, push the popup control.
                ArtistSearchList searchResults = SpotifyClientService.Client.SearchArtists(this.SearchTextBox.Text);

                ((SearchControlPopupViewModel)this.SearchControlPopup.DataContext).SearchResults = searchResults.ArtistItems.Items;
                this.SearchControlPopup.IsOpen = true;
            }
        }

        private void OnSearchControlPopupClosed(object sender, EventArgs e)
        {
            Artist selectedArtist = this.SearchControlPopup.SelectedItem;

            if (selectedArtist != null)
            {
                List<Artist> artistList = new List<Artist>() { selectedArtist };
                this.AtlasView.ViewModel.AddArtistsToHierarchy(artistList);

                this.AtlasView.UpdateNetwork();
            }
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

            if (this.SearchControlPopup.IsOpen == true)
            {
                this.SearchControlPopup.IsOpen = false;
            }
        }

        private void OnClearSearchTextButtonClick(object sender, RoutedEventArgs e)
        {
            this.SearchTextBox.Text = String.Empty;
            this.ClearSearchButton.Visibility = System.Windows.Visibility.Hidden;
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

                this.NavigationControl.OnCreatePlaylist(newPlaylist);
            }
        }

        /// <summary>
        /// Event raised when the playist selection has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaylistSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (this.StartUpHelper.Visibility == Visibility.Visible)
            {
                this.StartUpHelper.Visibility = Visibility.Hidden;
                this.AtlasView.ViewModel.IsVisible = true;
                this.PlaylistView.ViewModel.ShowTutorialInfo = false;
            }

            NavigationControl navigationControl = (NavigationControl)sender;
            NavigationViewModel navigationViewModel = (NavigationViewModel)navigationControl.DataContext;

            if (navigationViewModel.SelectedPlaylist != null)
            {
                IEnumerable<Artist> distinctArtists = navigationViewModel.SelectedPlaylist.Tracks.SelectMany(track => track.Track.Artists).Distinct().Select(artist => artist);

                this.AtlasView.ViewModel.PlaylistViewModel = (PlaylistViewModel)this.PlaylistView.DataContext;
                this.AtlasView.ViewModel.PlaylistViewModel.UpdatePlaylist(navigationViewModel.SelectedPlaylist); //TODO: Temporary -- need a better way to communicate across controls.

                this.AtlasView.ViewModel.CreateHierarchy(distinctArtists);
                this.AtlasView.UpdateNetwork();
            }
        }

        private void OnPlaylistTrackSelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectionChangedEventArgs args = (SelectionChangedEventArgs)e.OriginalSource;
            DataGrid sourceDataGrid = (DataGrid)args.Source;

            List<Spotify.Model.PlaylistTrack> selectedTracks = new List<Spotify.Model.PlaylistTrack>();
            foreach (Spotify.Model.PlaylistTrack selectedTrack in sourceDataGrid.SelectedItems)
            {
                selectedTracks.Add(selectedTrack);
            }

            IEnumerable<Artist> distinctArtists = selectedTracks.SelectMany(playlistTrack => playlistTrack.Track.Artists).Distinct().Select(artist => artist);
            this.AtlasView.ViewModel.SelectArtistNodes(distinctArtists);
        }

        private void OnRegenerateNetwork(object sender, RoutedEventArgs e)
        {
            IEnumerable<Artist> distinctArtists = this.PlaylistView.ViewModel.PlaylistTracks.SelectMany(track => track.Track.Artists).Distinct().Select(artist => artist);

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
            AddTracks(targetNodeViewModel);
        }

        private void OnFollowArtistClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel targetNodeViewModel = (NodeViewModel)(e.OriginalSource);

            if (!SpotifyCacheService.IsArtistFollowed(targetNodeViewModel.ArtistViewModel.Artist))
            {
                SpotifyCacheService.FollowArtist(targetNodeViewModel.ArtistViewModel.Artist);
            }
        }

        private void OnUnfollowArtistClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel targetNodeViewModel = (NodeViewModel)(e.OriginalSource);

            if (SpotifyCacheService.IsArtistFollowed(targetNodeViewModel.ArtistViewModel.Artist))
            {
                SpotifyCacheService.UnfollowArtist(targetNodeViewModel.ArtistViewModel.Artist);
            }
        }
        #endregion

        #region Login Methods
        private bool? ShowLoginWindow()
        {
            if (!SpotifyClientService.Client.HasAuthorizationAccess())
            {   
                _openLoginWindow = new LoginWindow();
                _openLoginWindow.ShowDialog();
                return _openLoginWindow.DialogResult;
            }

            return true;
        }

        private void Login()
        {
            if (ShowLoginWindow() == true)
            {
                this.PlaylistView.Initialize();
                this.NavigationControl.Initialize();

                _userCache = UserCache.Load(SpotifyClientService.User.Id);

                GetNewsFeed(_userCache);

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

        #region News Feed
        private void GetNewsFeed(UserCache userCache)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                AlbumType filter = AlbumType.Album | AlbumType.Single;
                DateTime cutoff = DateTime.Now.AddMonths(-3);
                int maxSuggestions = 1;

                FollowedArtistList followedArtists = SpotifyCacheService.GetFollowedArtists();
                List<NewsFeedItem> newsItems = new List<NewsFeedItem>();
    
                foreach (Artist followedArtist in followedArtists.ArtistItems.Items)
                {
                    //Find if there's any new content available from this artist.
                    AlbumInfoList albums = SpotifyClientService.Client.GetArtistAlbums(followedArtist, filter);

                    foreach (AlbumInfo album in albums.Items)
                    {
                        if (userCache.SeenNewsItem(album.ID)) continue;

                        Album albumInfo = SpotifyClientService.Client.GetAlbum(album);

                        if (albumInfo.ReleaseDate > cutoff)
                        {
                            newsItems.Add(new NewsFeedItem(followedArtist, albumInfo));
                            if (newsItems.Count >= maxSuggestions)
                                break;
                        }
                        else
                        {
                            //Assume that albums are returned by Spotify by release date, descending.
                            //If we miss the cutoff, skip out.
                            break;
                        }
                    }

                    if (newsItems.Count >= maxSuggestions)
                        break;
                }

                if (newsItems.Any())
                {
                    Dispatcher.Invoke(() =>
                    {
                        //Display a popup.
                        this.NewsFeedPopup.DataContext = new NewsFeedViewModel(newsItems, userCache);
                        this.NewsFeedPopup.Width = this.RenderSize.Width * 0.8;
                        this.NewsFeedPopup.Height = this.RenderSize.Height * 0.8;
                        this.NewsFeedPopup.IsOpen = true;
                    });
                }
            });
        }

        private void OnNotificationClick(object sender, RoutedEventArgs e)
        {
            GetNewsFeed(_userCache);
        }

        private void OnNewsFeedAddToPlaylist(object sender, RoutedEventArgs e)
        {
            AddToPlaylistEventArgs playlistArgs = (AddToPlaylistEventArgs)e;
            string discoveryPlaylistName = "Sound Atlas - Discovery";
            Playlist discoveryPlaylist = this.PlaylistView.ViewModel.Playlists.Where(playlist => playlist.Name.Equals(discoveryPlaylistName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (discoveryPlaylist == null)
            {
                //TODO: Prompt users.

                //Create the playlist in Spotify first.
                Playlist newPlaylist = SpotifyClientService.Client.CreatePlaylist(discoveryPlaylistName, SpotifyClientService.User.Id);

                this.NavigationControl.OnCreatePlaylist(newPlaylist);
            }
            else
            {
                this.NavigationControl.SelectPlaylist(discoveryPlaylist);
            }

            this.PlaylistView.ViewModel.AddAlbum(playlistArgs.Album);        
 
            this.NewsFeedPopup.SetNotificationText(String.Format("{0} has been added to the {1} playlist.", playlistArgs.Album.Name, discoveryPlaylistName));
        }
        #endregion

        #region Recommendations
        private void OnRecommendButtonClick(object sender, RoutedEventArgs e)
        {
            Playlist selectedPlaylist = (Playlist)this.NavigationControl.ViewModel.SelectedPlaylist;
            if (selectedPlaylist == null)
                return;

            RecommendationEngine engine = new RecommendationEngine();
            Artist recommendedArtist = engine.Recommend(SpotifyClientService.Client, selectedPlaylist);

            RecommendationWindow recommendationWindow = new RecommendationWindow(recommendedArtist);
            bool? result = recommendationWindow.ShowDialog();

            if (result.Value == true)
            {
                NodeViewModel targetNodeViewModel = this.AtlasView.ViewModel.FindNodeOfArtist(recommendedArtist);

                if (targetNodeViewModel == null)
                {
                    this.AtlasView.ViewModel.AddArtistsToHierarchy(new List<Artist>() { recommendedArtist });
                    this.AtlasView.UpdateNetwork();

                    targetNodeViewModel = this.AtlasView.ViewModel.FindNodeOfArtist(recommendedArtist);
                } 

                AddTracks(targetNodeViewModel);
            }
        }
        #endregion

        private void PlaylistView_PlaylistTrackSelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        #region Application Options
        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            //TODO: Change image based on state.

            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    

        private void OnApplicationBannerMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left
                && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }        
        #endregion Application Options


    }
}
