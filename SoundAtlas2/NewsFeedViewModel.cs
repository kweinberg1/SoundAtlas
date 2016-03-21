namespace SoundAtlas2
{
    using System.Collections.Generic;
    using System.Linq;
    using SoundAtlas2.Model;

    public class NewsFeedViewModel : ViewModelBase
    {
        #region Properties
        private readonly UserCache _cache;
        private int _index;

        private string _notificationPopupText;
        public string NotificationPopupText
        {
            get { return _notificationPopupText; }
            set
            {
                _notificationPopupText = value;
                NotifyPropertyChanged();
            }
        }

        private List<NewsFeedItem> _newsItems;
        public List<NewsFeedItem> NewsItems
        {
            get { return _newsItems; }
            set 
            { 
                _newsItems = value; 
                NotifyPropertyChanged(); 
            }
        }
        #endregion

        #region Constructors
        public NewsFeedViewModel()
        {
            NewsItems = new List<NewsFeedItem>();
            _cache = null;
        }

        public NewsFeedViewModel(List<NewsFeedItem> newsItems, UserCache userCache)
        {
            NewsItems = newsItems;
            _cache = userCache;

            if (NewsItems.Any())
                _cache.LogViewedNewsItem(NewsItems[_index].Album.ID);
        }
        #endregion

        #region Public Methods
        public NewsFeedItem GetPreviousNewsFeedItem()
        {
            _index = (_index - 1) % NewsItems.Count;
            if (_index < 0)
                _index = _index + NewsItems.Count;

            _cache.LogViewedNewsItem(NewsItems[_index].Album.ID);
            return NewsItems[_index]; 
        }

        public NewsFeedItem GetNextNewsFeedItem()
        {
            _index = (_index + 1) % NewsItems.Count;
            _cache.LogViewedNewsItem(NewsItems[_index].Album.ID);
            return NewsItems[_index]; 
        }

        public NewsFeedItem GetCurrentNewsFeedItem()
        {
            _cache.LogViewedNewsItem(NewsItems[_index].Album.ID);
            return NewsItems[_index]; 
        }
        #endregion
    }
}
