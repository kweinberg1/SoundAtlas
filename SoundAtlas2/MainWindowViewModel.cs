namespace SoundAtlas2
{
    using System;
    using SoundAtlas2.Model;

    /// <summary>
    /// The view-model for the main window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Properties
        private const String _applicationName = "Sound Atlas";
        public String ApplicationName => _applicationName;

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
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            
        }
        #endregion 
    }
}
