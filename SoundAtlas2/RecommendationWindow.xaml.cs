namespace SoundAtlas2
{
    using System.Windows;
    using Spotify.Model;

    /// <summary>
    /// Interaction logic for RecommendationWindow.xaml
    /// </summary>
    public partial class RecommendationWindow : Window
    {
        #region Properties
        private Artist _artist;
        #endregion

        #region Constructors
        public RecommendationWindow(Artist artist)
        {
            _artist = artist;

            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.WebBrowserControl.Navigate(_artist.ExternalURLs["spotify"]);
        }

        private void OnYesButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnNoButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        #endregion
    }
}
