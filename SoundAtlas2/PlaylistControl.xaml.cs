using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

using Spotify;
using Spotify.Model;

namespace SoundAtlas2
{
    /// <summary>
    /// Interaction logic for PlaylistControl.xaml
    /// </summary>
    public partial class PlaylistControl : UserControl
    {
        #region Properties
        private PlaylistViewModel _viewModel;
        public event RoutedEventHandler PlaylistTrackSelectionChanged
        {
            add { AddHandler(PlaylistTrackSelectionChangedEvent, value); }
            remove { RemoveHandler(PlaylistTrackSelectionChangedEvent, value); }
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public PlaylistViewModel ViewModel
        {
            get
            {
                return (PlaylistViewModel)DataContext;
            }
        }
        #endregion

        #region Events
        public readonly RoutedEvent PlaylistTrackSelectionChangedEvent = EventManager.RegisterRoutedEvent("PlaylistTrackSelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlaylistControl));
        #endregion


        public PlaylistControl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            _viewModel = new PlaylistViewModel();
            this.DataContext = _viewModel;
        }
        
        private void RaiseTrackSelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(PlaylistTrackSelectionChangedEvent, e);
            RaiseEvent(newEventArgs);
        }

        private void OnPlaylistListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseTrackSelectionChangedEvent(sender, e);
        }
    }
}
