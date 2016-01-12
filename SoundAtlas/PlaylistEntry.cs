using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundAtlas
{
    public class PlaylistEntry
    {
        #region Properties
        private String _artist;
        public String Artist
        {
            get { return _artist; }
            set { _artist = value; }
        }

        private String _songTitle;
        public String SongTitle
        {
            get { return _songTitle; }
            set { _songTitle = value; }
        }
        #endregion

        #region Constructors
        public PlaylistEntry(String artist, String songTitle)
        {
            Artist = artist;
            SongTitle = songTitle;
        }
        #endregion
    }

}
