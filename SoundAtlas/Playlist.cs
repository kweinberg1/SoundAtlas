using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundAtlas
{
    public class Playlist
    {
        #region Properties
        private String _name;
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private List<PlaylistEntry> _entries;
        public List<PlaylistEntry> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }

        private String _id;
        public String ID
        {
            get { return _id; }
        }

        private String _userID;
        public String UserID
        {
            get { return _userID;
        }
        #endregion

        #region Constructors
        public Playlist(String name)
        {
            _name = name;
            _entries = new List<PlaylistEntry>();
            _id = "4HuVr4Cle98wmsAcztB2Gw";
            _userID = "levikins";
        }
        #endregion
    }
}
