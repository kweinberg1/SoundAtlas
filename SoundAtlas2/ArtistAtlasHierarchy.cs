namespace SoundAtlas2
{
    using System.Collections.Generic;
    using SoundAtlas2.Model;
    using Spotify;
    using Spotify.Model;

    public class ArtistAtlasHierarchy : AtlasHierarchy
    {
        public ArtistAtlasHierarchy(SpotifyClient client, IEnumerable<Artist> artists, AtlasViewOptions viewOptions)
            : base(client, viewOptions) {

            foreach (Artist artist in artists)
            {
                GenerateTree(artist);
            }
        }

        /// <summary>
        /// Adds an artist to the root hierarchy.
        /// </summary>
        /// <param name="artistName"></param>
        public override void AddRootNode(object obj) {
            Artist artist = (Artist) obj;
            GenerateTree(artist);
        }
        
        /// <summary>
        /// Generates the tree from the root artist.
        /// </summary>
        /// <param name="artist"></param>
        protected override void GenerateTree(object obj)
        {
            Artist artist = (Artist)obj;
            if (NodeDictionary.ContainsKey(artist.Name))
                return;

            ArtistViewModel rootViewModel = new ArtistViewModel(artist);
            ArtistHierarchyNode root = new ArtistHierarchyNode(rootViewModel, this, null, 0);
            RootNodes.Add(root);
            NodeDictionary.Add(artist.Name, root);

            ArtistGroup relatedArtistList = Client.GetRelatedArtists(artist.ID);

            //First determine which artists are new; add those.
            List<Artist> newArtists = new List<Artist>();
            int nodesAdded = 0;

            foreach (Artist relatedArtist in relatedArtistList.Artists)
            {
                if (NodeDictionary.ContainsKey(relatedArtist.Name))
                    continue;

                if (nodesAdded < AddChildrenLimit)
                {
                    newArtists.Add(relatedArtist);

                    ArtistViewModel artistViewModel = new ArtistViewModel(relatedArtist);
                    ArtistHierarchyNode relatedArtistNode = new ArtistHierarchyNode(artistViewModel, this, root, 1);
                    root.Children.Add(relatedArtistNode);
                    NodeDictionary.Add(relatedArtist.Name, relatedArtistNode);

                    nodesAdded++;
                }
                else
                {
                    break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Returns whether the sub-tree has been fully expanded in the hierarchy.</returns>
        public override void GenerateSubTree(IHierarchyNode node) {
            ArtistHierarchyNode subTreeNode = (ArtistHierarchyNode) node;
            ArtistGroup relatedArtistList = Client.GetRelatedArtists(subTreeNode.ArtistViewModel.Artist.ID);

            //First determine which artists are new; add those.
            int nodesAdded = 0;
            foreach (Artist relatedArtist in relatedArtistList.Artists)
            {
                if (NodeDictionary.ContainsKey(relatedArtist.Name))
                    continue;

                if (nodesAdded < AddChildrenLimit)
                {
                    ArtistViewModel artistViewModel = new ArtistViewModel(relatedArtist);
                    ArtistHierarchyNode relatedArtistNode = new ArtistHierarchyNode(artistViewModel, this, subTreeNode, subTreeNode.Level + 1);
                    subTreeNode.Children.Add(relatedArtistNode);
                    NodeDictionary.Add(relatedArtist.Name, relatedArtistNode);
                    nodesAdded++;
                }
                else
                {
                    return;
                }
            }
        }
    }
}
