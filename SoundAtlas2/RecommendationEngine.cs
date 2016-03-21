namespace SoundAtlas2
{
    using System.Collections.Generic;
    using System.Linq;

    using Spotify;
    using Spotify.Model;

    internal class RecommendationEngine
    {
        #region Methods
        public Artist Recommend(SpotifyClient client, Playlist playlist)
        {
            //Find a common ancestor for a portion of the artists in the playlist.
            IEnumerable<Track> playlistEntries = playlist.Tracks.Select(track => track.Track);

            Dictionary<string, ArtistGroup> cachedRelatedArtists = new Dictionary<string, ArtistGroup>();

            Dictionary<string, RecommendationCriteria> relatedArtistMap = new Dictionary<string, RecommendationCriteria>();
            playlistEntries.ToList().ForEach(track =>
            {
                ArtistGroup relatedArtists = null;
                if (!cachedRelatedArtists.ContainsKey(track.Artists.First().ID))
                {
                    relatedArtists = client.GetRelatedArtists(track.Artists.First().ID); //Handle only the first artist for now...
                    cachedRelatedArtists.Add(track.Artists.First().ID, relatedArtists);
                }
                else
                {
                    relatedArtists = cachedRelatedArtists[track.Artists.First().ID];
                }

                relatedArtists.Artists.ForEach(relatedArtist =>
                {
                    if (!relatedArtistMap.ContainsKey(relatedArtist.ID))
                    {
                        RecommendationCriteria recommendationCriteria = new RecommendationCriteria();
                        relatedArtistMap.Add(relatedArtist.ID, recommendationCriteria);
                        recommendationCriteria.TargetedArtist = relatedArtist;
                    }

                    if (!relatedArtistMap[relatedArtist.ID].RelatedArtists.Where(artist => artist.ID == track.Artists.First().ID).Any())
                        relatedArtistMap[relatedArtist.ID].RelatedArtists.Add(track.Artists.First());

                    relatedArtistMap[relatedArtist.ID].Weight++;
                });
            });

            //Other ideas:
            //Recommend by the playlist contents.
            //  Evaluate whether the playlist is a collection of like-artists or individual songs of a particular type (e.g. energetic, fast?)
            //Recommend by the artist selected.
            //
            //Recommend something completely new.  
            //Recommend a set of brand-new songs.
            //  Analyze all changelists to get an idea of what a person likes in particular.  
            //  Easily solution is to find artists that they don't have that are similar.
            //  Could also randomly find songs from various areas:
            //      The current Top 10-40 on the charts.
            //      The list of a band that is playing near you live.
            //      Find a whole smattering of a different genre (trip hop, hip hop, grunge alternative).
            //Recommend by a specific song.
            //      Find songs that were popular at the same time (may not need to be part of the same genre).

            IOrderedEnumerable<KeyValuePair<string, RecommendationCriteria>> orderedDictionary = relatedArtistMap.OrderByDescending(keyValuePair => keyValuePair.Value.Weight)
                                                                                                                    .ThenByDescending(keyValuePair => keyValuePair.Value.RelatedArtists.Count());
            KeyValuePair<string, RecommendationCriteria> recommendedEntry = orderedDictionary.First();
            return recommendedEntry.Value.TargetedArtist;
        }
        #endregion
    }
}
