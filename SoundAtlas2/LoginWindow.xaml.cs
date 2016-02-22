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

namespace SoundAtlas2
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool _isClosed;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _isClosed = false;

            Task<bool> listenerTask = Task<bool>.Run(() => ListenForAuthorization());
            listenerTask.ContinueWith((task) =>
                {
                    //Invoke the UI element changes in the main thread.
                    this.Dispatcher.Invoke(() =>
                        {
                            Spotify.SpotifyClientService.Login();

                            if (!_isClosed)
                            {
                                DialogResult = task.Result;
                                Close();
                            }
                        });
                });
            LaunchAuthorization();
        }

        private void LaunchAuthorization()
        {
            String navigationUrl = Spotify.SpotifyClientService.Client.GetAuthorizationUrl();
            this.WebBrowserControl.Navigate(navigationUrl);
        }

        private bool ListenForAuthorization()
        {
            Task<bool> authorizationListener = Spotify.SpotifyClientService.Client.ListenForAuthorization();
            authorizationListener.Wait();
            return authorizationListener.Result;
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            _isClosed = true;
        }
    }
}
