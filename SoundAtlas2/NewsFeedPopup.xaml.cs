namespace SoundAtlas2
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Interaction logic for NewsFeedPopup.xaml
    /// </summary>
    public partial class NewsFeedPopup : Popup
    {
        #region Events
        public readonly RoutedEvent AddToPlaylistEvent = EventManager.RegisterRoutedEvent("AddToPlaylist", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));

        public event RoutedEventHandler AddToPlaylist
        {
            add { AddHandler(AddToPlaylistEvent, value); }
            remove { RemoveHandler(AddToPlaylistEvent, value); }
        }
        #endregion

        #region Properties
        public NewsFeedViewModel ViewModel
        {
            get { return (NewsFeedViewModel)this.DataContext; }
        }
        #endregion

        #region Constructors
        public NewsFeedPopup()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        public void SetNotificationText(string text)
        {
            ViewModel.NotificationPopupText = text;
        }
        #endregion

        #region Event Handlers
        private void Panel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            NewsFeedViewModel viewModel = (NewsFeedViewModel)this.DataContext;

            if (viewModel != null && viewModel.NewsItems.Any())
            {
                NewsFeedItem newsFeedItem = viewModel.GetCurrentNewsFeedItem();
                this.WebBrowserControl.Navigate(newsFeedItem.Album.ExternalUrls["spotify"]);
            }

            SetNotificationText(string.Empty);
        }

        private void OnLeftButtonClick(object sender, RoutedEventArgs e)
        {
            NewsFeedViewModel viewModel = (NewsFeedViewModel)this.DataContext;

            if (viewModel.NewsItems.Any())
            {
                NewsFeedItem newsFeedItem = viewModel.GetPreviousNewsFeedItem();

                this.WebBrowserControl.Navigate(newsFeedItem.Album.ExternalUrls["spotify"]);
            }

            SetNotificationText(string.Empty);
        }

        private void OnRightButtonClick(object sender, RoutedEventArgs e)
        {
            NewsFeedViewModel viewModel = (NewsFeedViewModel)this.DataContext;

            if (viewModel.NewsItems.Any())
            {
                NewsFeedItem newsFeedItem = viewModel.GetNextNewsFeedItem();

                this.WebBrowserControl.Navigate(newsFeedItem.Album.ExternalUrls["spotify"]);
            }

            SetNotificationText(string.Empty);
        }

        private void OnAddToPlaylistClick(object sender, RoutedEventArgs e)
        {
            NewsFeedViewModel viewModel = (NewsFeedViewModel)this.DataContext;

            if (viewModel != null)
            {
                NewsFeedItem newsFeedItem = viewModel.GetCurrentNewsFeedItem();
                if (!newsFeedItem.Added)
                {
                    AddToPlaylistEventArgs eventArgs = new AddToPlaylistEventArgs(AddToPlaylistEvent, newsFeedItem.Album);
                    RaiseEvent(eventArgs);

                    newsFeedItem.Added = true;
                }
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }
        #endregion
    }
}
