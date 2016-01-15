using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoundAtlas2.Model;
using Spotify.Model;

namespace SoundAtlas2
{
    public class SearchControlViewModel : ViewModelBase
    {
        #region Constructors
        public SearchControlViewModel()
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
