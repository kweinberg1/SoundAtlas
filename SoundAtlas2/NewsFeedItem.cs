using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spotify.Model;

namespace SoundAtlas2
{
    public class NewsFeedItem
    {
        public Artist Artist
        {
            get;
            private set;
        }

        public Album Album
        {
            get;
            private set;
        }

        public bool Added
        {
            get;
            set;
        }

        public NewsFeedItem(Artist artist, Album album)
        {
            Artist = artist;
            Album = album;
            Added = false;
        }
    }
}
