using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SoundAtlas2.Model;
using Spotify.Model;

namespace SoundAtlas2
{
    public class NewsFeedViewModel : ViewModelBase
    {
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

        private int _index;
        private UserCache _cache;

        public NewsFeedViewModel()
        {
            NewsItems = new List<NewsFeedItem>();
            
            _index = 0;
        }

        public NewsFeedViewModel(List<NewsFeedItem> newsItems, UserCache userCache)
        {
            NewsItems = newsItems;
            _cache = userCache;

            if (NewsItems.Any())
                _cache.LogViewedNewsItem(NewsItems[_index].Album.ID);
        }

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
    }
}
