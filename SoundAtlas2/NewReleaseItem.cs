namespace SoundAtlas2
{
    using Spotify.Model;

    public class NewReleaseItem
    {
        #region Properties
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
        #endregion

        #region Constructors
        public NewReleaseItem(Artist artist, Album album)
        {
            Artist = artist;
            Album = album;
            Added = false;
        }
        #endregion
    }
}
