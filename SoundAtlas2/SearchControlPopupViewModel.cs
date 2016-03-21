namespace SoundAtlas2
{
    using System.Collections.Generic;
    using SoundAtlas2.Model;
    using Spotify.Model;

    public class SearchControlPopupViewModel : ViewModelBase
    {
        #region Constructors
        public SearchControlPopupViewModel()
        {
            _searchResults = new List<Artist>();
        }
        #endregion

        #region Properties
        private List<Artist> _searchResults;
        public List<Artist> SearchResults
        {
            get { return _searchResults; }
            set
            {
                _searchResults = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
