namespace SoundAtlas2.Model
{
    using Spotify.Model;

    public interface IPlaylistView
    {
        int AddArtistTracks(Artist artist);

        int GetArtistTrackCount(Artist artist);
    }
}
