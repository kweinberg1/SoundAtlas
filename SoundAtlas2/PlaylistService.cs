namespace SoundAtlas2
{
    using System.Linq;
    using SoundAtlas2.Model;
    using Spotify.Model;

    internal class PlaylistService
    {
        private static PlaylistViewModel _playlistViewModel;

        public static void Initialize(PlaylistViewModel playlistViewModel) {
            _playlistViewModel = playlistViewModel;
        }
        
        /// <summary>
        /// Returns the number of tracks associated with the given artist.
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public static int GetArtistTrackCount(Artist artist)
        {
            if (_playlistViewModel.Playlist != null) {
                return _playlistViewModel.Playlist.Tracks.SelectMany(track => track.Track.Artists).Count(trackArtist => trackArtist.ID == artist.ID);
            }

            return 0;
        }

        public static IPlaylistView GetCurrentPlaylist()
        {
            return (IPlaylistView)_playlistViewModel;
        }
    }
}
