using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Spotify.Model;

namespace SoundAtlas
{
    [DebuggerDisplay("ArtistViewModel {Artist.Name}")]
    public class ArtistViewModel : ViewModelBase
    {
        #region Properties
        private Artist _artist;
        public Artist Artist
        {
            get { return _artist; }
            private set { _artist = value; }
        }

        public ArtistHierarchy.Node HierarchyNode
        {
            get;
            private set;
        }

        public String Name
        {
            get { return Artist.Name; }
        }

        private bool _selected;
        public bool IsSelected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                NotifyPropertyChanged();
            }
        }

        private bool _flagged;
        public bool IsFlagged
        {
            get { return _flagged; }
            set
            {
                _flagged = value;
                NotifyPropertyChanged();
            } 
        }

        public String Details
        {
            get
            {
                String genres = String.Join(", ", Artist.Genres);
                String details = genres;
                return details;
            }
        }

        public Point Location
        {
            get;
            private set;
        }
        #endregion

        #region Constructor
        public ArtistViewModel(Artist artist)
        {
            _artist = artist;
            _selected = false;
        }
        #endregion

        #region Public Methods
        public void SetHierarchyNode(ArtistHierarchy.Node node)
        {
            HierarchyNode = node;
        }

        public void SetLocation(Point location)
        {
            Location = location;
        }
        #endregion
    }
}
