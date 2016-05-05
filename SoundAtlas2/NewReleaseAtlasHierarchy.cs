namespace SoundAtlas2
{
    using System.Collections.Generic;
    using SoundAtlas2.Model;
    using Spotify;

    public class NewReleaseAtlasHierarchy : AtlasHierarchy
    {
        public NewReleaseAtlasHierarchy(SpotifyClient client, IEnumerable<NewReleaseItem> newReleases, AtlasViewOptions viewOptions)
            : base(client, viewOptions) {
            foreach (NewReleaseItem newReleaseItem in newReleases) {
                GenerateTree(newReleaseItem);
            }
        }

        public override void AddRootNode(object obj)
        {
            NewReleaseItem newRelease = (NewReleaseItem)obj;
            GenerateTree(newRelease);
        }

        public override void GenerateSubTree(IHierarchyNode node) {
            //New Release Items do not have sub-trees.
            return;
        }

        protected override void GenerateTree(object obj) {

            NewReleaseItem newReleaseItem = (NewReleaseItem) obj;
            NewReleaseHierarchyNode root = new NewReleaseHierarchyNode(newReleaseItem, this);
            RootNodes.Add(root);
            NodeDictionary.Add(root.NewReleaseItem.Album.Name, root);
        }
    }
}
