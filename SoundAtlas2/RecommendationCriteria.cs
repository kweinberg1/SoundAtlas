using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spotify.Model;

namespace SoundAtlas2
{
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
