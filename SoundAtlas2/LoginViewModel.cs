using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoundAtlas2.Model;
namespace SoundAtlas2
{
    public class LoginViewModel : ViewModelBase
    {
        #region Properties
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

        private readonly String NotLoggedInString = "Not Logged In";
        #region Constructors
        public LoginViewModel()
        {
            AccountName = NotLoggedInString;
            MusicService = MusicService.None;
        }
        #endregion
    }
}
