namespace SoundAtlas2
{
    using System.Collections.Generic;
    using Spotify.Model;

    public sealed class RecommendationCriteria
    {
        public Artist TargetedArtist;
        public List<Artist> RelatedArtists;
        public int Weight;

        public RecommendationCriteria()
        {
            TargetedArtist = null;
            RelatedArtists = new List<Artist>();
            Weight = 0;
        }
    }
}
