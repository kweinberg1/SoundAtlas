namespace SoundAtlas2
{
    using System.Diagnostics;
    using System.Linq;
    using SoundAtlas2.Model;
    using Spotify;
    using Spotify.Model;

    [DebuggerDisplay("Node {ArtistViewModel.Artist.Name}")]
    public class ArtistHierarchyNode : IHierarchyNode
    {
        #region Properties
        public ArtistViewModel ArtistViewModel
        {
            get;
            private set;
        }

        #endregion

        public ArtistHierarchyNode(ArtistViewModel artistViewModel, AtlasHierarchy hierarchy, IHierarchyNode parent, int level)
            : base(hierarchy, parent, level)
        {
            ArtistViewModel = artistViewModel;
        }

        public bool IsSubTreeExpanded()
        {
            ArtistGroup relatedArtistList = SpotifyClientService.Client.GetRelatedArtists(ArtistViewModel.Artist.ID);

            //First determine which artists are new; add those.
            foreach (Artist relatedArtist in relatedArtistList.Artists)
            {
                if (Hierarchy.NodeExists(relatedArtist.Name))
                    continue;

                if (Children.Where(node => ArtistViewModel.Artist == relatedArtist).Any())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
