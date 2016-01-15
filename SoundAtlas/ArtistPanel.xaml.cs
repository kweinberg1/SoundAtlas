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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoundAtlas
{
    /// <summary>
    /// Interaction logic for ArtistPanel.xaml
    /// </summary>
    public partial class ArtistPanel : StackPanel
    {
        #region Properties
        public event RoutedEventHandler AddPopularTracks
        {
            add { AddHandler(AddPopularTracksEvent, value); }
            remove { RemoveHandler(AddPopularTracksEvent, value); }
        }
        #endregion

        #region Events
        public static readonly RoutedEvent AddPopularTracksEvent = EventManager.RegisterRoutedEvent("AddPopularTracks", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ArtistPanel));
        #endregion

        public ArtistPanel()
        {
            InitializeComponent();
        }

        private void RaiseAddPopularTracksEvent(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(AddPopularTracksEvent, e);
            RaiseEvent(newEventArgs);
        }

        private void CanExecuteAddPopularTracks(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.IsEnabled;
        }

        private void ExecuteAddPopularTracks(object sender, ExecutedRoutedEventArgs e)
        {
            ArtistViewModel artistViewModel = (ArtistViewModel)this.DataContext;
            if (artistViewModel.IsFlagged == false)
            {
                artistViewModel.IsFlagged = true;
                RaiseAddPopularTracksEvent(sender, e);
            }
        }
    }
}
