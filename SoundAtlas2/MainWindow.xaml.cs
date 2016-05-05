namespace SoundAtlas2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Model;
    using Spotify;
    using Spotify.Model;

    /// <summary>
    /// This is a Window that uses NetworkView to display a flow-chart.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private const String DefaultSearchText = "Search";

        private LoginWindow _openLoginWindow;
        private UserCache _userCache;
        private AtlasViewMode _atlasViewMode;
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event raised when the Window has loaded.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*OverviewWindow overviewWindow = new OverviewWindow();
            overviewWindow.Left = this.Left;
            overviewWindow.Top = this.Top + this.Height + 5;
            overviewWindow.Owner = this;
            overviewWindow.DataContext = this.AtlasView.ViewModel; // Pass the view model onto the overview window.
            overviewWindow.Show();
            */
        }
        #endregion 

        #region Search Event Handlers
        private void OnSearchPanelKeyDown(object sender, KeyEventArgs e)
        {
            TextBox searchPanelTextBox = (TextBox)sender;

            this.ClearSearchButton.Visibility = searchPanelTextBox.Text.Length > 0 ? Visibility.Visible : Visibility.Hidden;

            if (e.Key == Key.Enter)
            {
                //For now, push the popup control.
                ArtistSearchList searchResults = SpotifyClientService.Client.SearchArtists(this.SearchTextBox.Text);

                ((SearchControlPopupViewModel)this.SearchControlPopup.DataContext).SearchResults = searchResults.ArtistItems.Items;
                this.SearchControlPopup.IsOpen = true;
            }
        }

        /// <summary>
        /// Event handler when the search popup is closed.  Adds the artist to the atlas
        /// if there a valid selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchControlPopupClosed(object sender, EventArgs e)
        {
            Artist selectedArtist = this.SearchControlPopup.SelectedItem;

            if (selectedArtist == null)
                return;

            List<Artist> artistList = new List<Artist>() { selectedArtist };
            AtlasViewOptions options = new AtlasViewOptions(1);
            this.AtlasView.ViewModel.AddArtistsToHierarchy(artistList.AsReadOnly(), options);
            this.AtlasView.UpdateNetwork();
        }

        /// <summary>
        /// Event handler when the search panel gets focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchPanelFocus(object sender, RoutedEventArgs e)
        {
            TextBox searchControlTextBox = (TextBox) sender;

            if (searchControlTextBox.Text.Equals(DefaultSearchText))
            {
                searchControlTextBox.Text = String.Empty;
            }
        }

        /// <summary>
        /// Eveht handler when the search panel loses focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Event handler when the clear search text button is clicked.
        /// This will search the search box's text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            if (navigationViewModel.SelectedPlaylist != null) {
                _atlasViewMode = AtlasViewMode.PlaylistView;
                IEnumerable<Artist> distinctArtists = navigationViewModel.SelectedPlaylist.Tracks.SelectMany(track => track.Track.Artists).Distinct().Select(artist => artist);

                //Notify the playlist view model that the playlist has been updated.
                this.PlaylistView.ViewModel.UpdatePlaylist(navigationViewModel.SelectedPlaylist);

                AtlasViewOptions options = new AtlasViewOptions(1);
                this.AtlasView.ViewModel.CreatePlaylistHierarchy(distinctArtists.ToList().AsReadOnly(), options);
                this.AtlasView.UpdateNetwork();
            }
            else
            {
                IReadOnlyCollection<Artist> emptyList = new List<Artist>();
                AtlasViewOptions options = new AtlasViewOptions(1);
                this.AtlasView.ViewModel.CreatePlaylistHierarchy(emptyList, options);
                this.AtlasView.UpdateNetwork();
            }
        }
        
        /// <summary>
        /// Event handler when the playlist track selection has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaylistTrackSelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectionChangedEventArgs args = (SelectionChangedEventArgs)e.OriginalSource;
            DataGrid sourceDataGrid = (DataGrid)args.Source;

            List<Spotify.Model.PlaylistTrack> selectedTracks = sourceDataGrid.SelectedItems.OfType<PlaylistTrack>().ToList();

            IEnumerable<Artist> distinctArtists = selectedTracks.SelectMany(playlistTrack => playlistTrack.Track.Artists).Distinct().Select(artist => artist);
            this.AtlasView.ViewModel.SelectArtistNodes(distinctArtists);
        }

        /// <summary>
        /// Regenerates the entire network.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRegenerateNetwork(object sender, RoutedEventArgs e)
        {
            switch(_atlasViewMode)
            {
                case AtlasViewMode.FollowedArtistView:
                {
                    OnFollowedArtists(this.NavigationControl, e);
                }
                break;
                case AtlasViewMode.PlaylistView:
                {
                    OnPlaylistSelectionChanged(this.NavigationControl, e);
                }    
                break;
                case AtlasViewMode.NewReleasesView:
                {

                }
                break;
            }
        }
        #endregion

        #region Login Methods
        /// <summary>
        /// Displays the login window.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Displays the login window and will initialize the application with
        /// login credentials.
        /// </summary>
        private void Login() {
            if (ShowLoginWindow() == false)
                return;
            
            this.PlaylistView.Initialize();
            this.NavigationControl.Initialize();
            PlaylistService.Initialize(this.PlaylistView.ViewModel);

            _userCache = UserCache.Load(SpotifyClientService.User.Id);

            GetNewsFeed(_userCache);

            //TODO:  Support other music services.  This should be broken out into a separate UI action.
            LoginViewModel loginViewModel = (LoginViewModel)this.LoginControl.DataContext;
            loginViewModel.AccountName = SpotifyClientService.User.DisplayName;
            loginViewModel.MusicService = MusicService.Spotify;

            MainWindowViewModel mainWindowViewModel = (MainWindowViewModel)this.DataContext;
            mainWindowViewModel.IsLoggedIn = true;
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

        #region News Feed Methods
        /// <summary>
        /// Spawn a background task to find any new albums or tracks
        /// based on what the user is following.  A news feed item will
        /// popup if an item is found.
        /// </summary>
        /// <param name="userCache"></param>
        private void GetNewsFeed(UserCache userCache)
        {
            /*System.Threading.Tasks.Task.Run(() =>
            {
                AlbumType filter = AlbumType.Album | AlbumType.Single;
                DateTime cutoff = DateTime.Now.AddMonths(-3); //TODO: Data-drive this setting.
                int maxSuggestions = 1;  //TODO: Data-drive this setting.

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
            });*/
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
        private void OnRecommend(object sender, RoutedEventArgs e)
        {
            Playlist selectedPlaylist = (Playlist)this.NavigationControl.ViewModel.SelectedPlaylist;
            if (selectedPlaylist == null)
                return;

            RecommendationEngine engine = new RecommendationEngine();
            Artist recommendedArtist = engine.Recommend(SpotifyClientService.Client, selectedPlaylist);

            RecommendationWindow recommendationWindow = new RecommendationWindow(recommendedArtist);
            bool? result = recommendationWindow.ShowDialog();

            if (result != null && result.Value == true)
            {
                ArtistNetworkNodeViewModel targetNodeViewModel = this.AtlasView.ViewModel.FindNode<ArtistNetworkNodeViewModel>(recommendedArtist.ID);

                if (targetNodeViewModel == null) {
                    List<Artist> recommendedArtistList = new List<Artist>() {recommendedArtist};
                    AtlasViewOptions options = new AtlasViewOptions(1);
                    this.AtlasView.ViewModel.AddArtistsToHierarchy(recommendedArtistList.AsReadOnly(), options);
                    this.AtlasView.UpdateNetwork();

                    targetNodeViewModel = this.AtlasView.ViewModel.FindNode<ArtistNetworkNodeViewModel>(recommendedArtist.ID);
                }

                targetNodeViewModel.AddTracks();
            }
        }

        /// <summary>
        /// Populates the Atlas with all followed artists.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFollowedArtists(object sender, RoutedEventArgs e)
        {
            //Clear the Atlas, clear the playlist selection too.
            //Show all artists you're following.
            _atlasViewMode = AtlasViewMode.FollowedArtistView;

            //Clear the playlist selection.
            NavigationControl navigationControl = (NavigationControl)sender;
            navigationControl.SelectPlaylist(null);
            OnPlaylistSelectionChanged(navigationControl, e);

            //Add all followed artists to the hierarchy.
            FollowedArtistList followedArtists = SpotifyCacheService.GetFollowedArtists();
            AtlasViewOptions viewOptions = new AtlasViewOptions(0);
            this.AtlasView.ViewModel.CreateFollowedArtistHierarchy(followedArtists.ArtistItems.Items.AsReadOnly(), viewOptions);
            this.AtlasView.UpdateNetwork();
        }

        private void OnNewReleases(object sender, RoutedEventArgs e)
        {
            _atlasViewMode = AtlasViewMode.NewReleasesView;

            //Clear the playlist selection.
            NavigationControl navigationControl = (NavigationControl)sender;
            navigationControl.SelectPlaylist(null);
            OnPlaylistSelectionChanged(navigationControl, e);

            List<NewReleaseItem> newsItems = new List<NewReleaseItem>();

            //Add artists that have new releases.
            //System.Threading.Tasks.Task.Run(() =>
            {
                AlbumType filter = AlbumType.Album | AlbumType.Single;
                DateTime cutoff = DateTime.Now.AddMonths(-3); //TODO: Data-drive this setting.
                int maxSuggestions = 1;  //TODO: Data-drive this setting.

                FollowedArtistList followedArtists = SpotifyCacheService.GetFollowedArtists();
    
                foreach (Artist followedArtist in followedArtists.ArtistItems.Items)
                {
                    //Find if there's any new content available from this artist.
                    AlbumInfoList albums = SpotifyClientService.Client.GetArtistAlbums(followedArtist, filter);

                    foreach (AlbumInfo album in albums.Items)
                    {
                        //if (_userCache.SeenNewsItem(album.ID)) continue;

                        Album albumInfo = SpotifyClientService.Client.GetAlbum(album);

                        if (albumInfo.ReleaseDate > cutoff)
                        {
                            newsItems.Add(new NewReleaseItem(followedArtist, albumInfo));
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
                }

                if (newsItems.Any())
                {                    
                    Dispatcher.Invoke(() =>
                    {
                        //Display a popup.
                        //this.NewsFeedPopup.DataContext = new NewsFeedViewModel(newsItems, userCache);
                        //this.NewsFeedPopup.Width = this.RenderSize.Width * 0.8;
                        //this.NewsFeedPopup.Height = this.RenderSize.Height * 0.8;
                        //this.NewsFeedPopup.IsOpen = true;
                    });
                }
            }
            //);
            
            AtlasViewOptions viewOptions = new AtlasViewOptions(0);
            this.AtlasView.ViewModel.CreateNewReleaseHierarchy(newsItems.ToList().AsReadOnly(), viewOptions);
            this.AtlasView.UpdateNetwork();
        }
        #endregion

        #region Application Options
        /// <summary>
        /// Minimizes the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        /// <summary>
        /// Maximizes or restores the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Closes the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event handler for users exiting the application via the File menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExitMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event handler when users click on the top application bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
