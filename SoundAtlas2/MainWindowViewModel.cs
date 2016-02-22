using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using Utils;

using SoundAtlas2.Model;

namespace SoundAtlas2
{
    /// <summary>
    /// The view-model for the main window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get
            {
                return _isLoggedIn;
            }
            set
            {
                _isLoggedIn = value;
                NotifyPropertyChanged();
            }
        }
    }
}
