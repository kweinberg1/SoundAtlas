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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Spotify.Model;

namespace SoundAtlas2
{
    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControlPopup : Popup
    {
        public SearchControlPopup()
        {
            InitializeComponent();
        }

        #region Properties
        public Artist SelectedItem
        {
            get;
            set;
        }
        #endregion

        #region Methods
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;

            SelectedItem = (Artist)listBox.SelectedItem;
            
            this.IsOpen = false;
        }
        #endregion
    }
}
