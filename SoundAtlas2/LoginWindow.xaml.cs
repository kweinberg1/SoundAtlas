namespace SoundAtlas2
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Properties
        private bool _isClosed;
        #endregion

        #region Constructors
        public LoginWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
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

            //Listen for the authorization thread.
            LaunchAuthorization();
        }


        private void OnWindowClosed(object sender, EventArgs e)
        {
            _isClosed = true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Notifies the web control to prompt users to login.
        /// </summary>
        private void LaunchAuthorization()
        {
            String navigationUrl = Spotify.SpotifyClientService.Client.GetAuthorizationUrl();
            this.WebBrowserControl.Navigate(navigationUrl);
        }

        /// <summary>
        /// Function to spawn off a thread to listen to the authorization callback.
        /// </summary>
        /// <returns></returns>
        private bool ListenForAuthorization()
        {
            Task<bool> authorizationListener = Spotify.SpotifyClientService.Client.ListenForAuthorization();
            authorizationListener.Wait();
            return authorizationListener.Result;
        }
        #endregion
    }
}
