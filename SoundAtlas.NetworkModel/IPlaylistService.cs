namespace SoundAtlas2.Model
{
    using Spotify.Model;

    interface IPlaylistService
    {
        int GetArtistTrackCount(Artist artist);
        
        IPlaylistView GetCurrentPlaylist();
    }
}
