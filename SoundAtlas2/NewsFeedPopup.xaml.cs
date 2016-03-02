using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Spotify.Model;

namespace SoundAtlas2
{
    /// <summary>
    /// Interaction logic for NewsFeedPopup.xaml
    /// </summary>
    public partial class NewsFeedPopup : Popup
    {
        public readonly RoutedEvent AddToPlaylistEvent = EventManager.RegisterRoutedEvent("AddToPlaylist", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));

        public event RoutedEventHandler AddToPlaylist
        {
            add { AddHandler(AddToPlaylistEvent, value); }
            remove { RemoveHandler(AddToPlaylistEvent, value); }
        }

        public NewsFeedViewModel ViewModel
        {
            get { return (NewsFeedViewModel)this.DataContext; }
        }

        public NewsFeedPopup()
        {
            InitializeComponent();
        }

        public void SetNotificationText(string text)
        {
            ViewModel.NotificationPopupText = text;
        }

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
    }

    public class AddToPlaylistEventArgs : RoutedEventArgs
    {
        public Album Album { get; private set; }

        public AddToPlaylistEventArgs(RoutedEvent e, Album album)
            : base(e)
        {
            Album = album;
        }
    }
}
