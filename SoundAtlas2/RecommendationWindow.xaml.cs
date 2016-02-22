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
using System.Windows.Shapes;

using Spotify.Model;

namespace SoundAtlas2
{
    /// <summary>
    /// Interaction logic for RecommendationWindow.xaml
    /// </summary>
    public partial class RecommendationWindow : Window
    {
        private Artist _artist;

        public RecommendationWindow(Artist artist)
        {
            _artist = artist;

            InitializeComponent();
        }

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
    }
}
