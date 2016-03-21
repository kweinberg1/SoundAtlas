namespace SoundAtlas2
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Spotify.Model;


    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControlPopup : Popup
    {
        #region Constructors
        public SearchControlPopup()
        {
            InitializeComponent();
        }
        #endregion

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
