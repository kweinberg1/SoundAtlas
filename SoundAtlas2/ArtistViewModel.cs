namespace SoundAtlas2
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using SoundAtlas2.Model;
    using Spotify.Model;

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
        /*public void SetHierarchyNode(AtlasHierarchy.HierarchyNode node)
        {
            HierarchyNode = node;
        }*/

        public void SetLocation(Point location)
        {
            Location = location;
        }
        #endregion
    }
}
