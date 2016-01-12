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
    public class SearchEventArgs : RoutedEventArgs
    {
        public String SearchTerm;

        public SearchEventArgs(String searchTerm)
        {
            SearchTerm = searchTerm;
        }
    }

    /// <summary>
    /// Interaction logic for SearchPanel.xaml
    /// </summary>
    public partial class SearchPanel : UserControl
    {
        #region Events
        public static readonly RoutedEvent SearchExecutedEvent = EventManager.RegisterRoutedEvent("SearchExecuted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchPanel)); 
        #endregion

        #region Properties
        public event RoutedEventHandler SearchExecuted
        {
            add { AddHandler(SearchExecutedEvent, value); }
            remove { RemoveHandler(SearchExecutedEvent, value); }
        }
        #endregion

        public SearchPanel()
        {
            InitializeComponent();
        }

        private void RaiseSearchExecutedEvent(object sender, String searchText)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(SearchExecutedEvent, searchText);
            RaiseEvent(newEventArgs);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = (TextBox)sender;
                RaiseSearchExecutedEvent(sender, textBox.Text);
            }
        }
    }
}
