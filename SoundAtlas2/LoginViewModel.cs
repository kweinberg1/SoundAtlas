namespace SoundAtlas2
{
    using System;
    using SoundAtlas2.Model;

    public class LoginViewModel : ViewModelBase
    {
        #region Properties
        private readonly String NotLoggedInString = "Not Logged In"; //TODO:  Localize.
        private String _accountName;
        public String AccountName
        {
            get
            {
                return _accountName;
            }
            set
            {
                _accountName = value;
                NotifyPropertyChanged();
            }
        }

        private MusicService _musicService;
        public MusicService MusicService
        {
            get
            {
                return _musicService;
            }
            set
            {
                _musicService = value;
                NotifyPropertyChanged();
            }
        }
        #endregion Properties

        #region Constructors
        public LoginViewModel()
        {
            AccountName = NotLoggedInString;
            MusicService = MusicService.None;
        }
        #endregion
    }
}
