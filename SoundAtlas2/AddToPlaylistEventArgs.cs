namespace SoundAtlas2
{
    using System.Windows;
    using Spotify.Model;

    public class AddToPlaylistEventArgs : RoutedEventArgs
    {
        public Album Album { get; private set; }

        public AddToPlaylistEventArgs(RoutedEvent e, Album album)
            : base(e)
        {
            Album = album;
        }
    }
}
