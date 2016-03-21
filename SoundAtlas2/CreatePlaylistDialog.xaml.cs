namespace SoundAtlas2
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for CreatePlaylistDialog.xaml
    /// </summary>
    public partial class CreatePlaylistDialog : Window
    {
        #region Properties
        public String PlaylistName
        {
            get;
            private set;
        }
        #endregion 

        #region Constructors
        public CreatePlaylistDialog()
        {
            InitializeComponent();

            PlaylistName = null;
        }
        #endregion

        #region Event Handlers
        private void OnOKButtonClick(object sender, RoutedEventArgs e)
        {
            PlaylistName = PlaylistNameTextBox.Text;
            this.DialogResult = true;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            PlaylistName = null;
            this.DialogResult = false;
        }
        #endregion
    }
}
