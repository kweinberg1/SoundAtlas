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
