namespace SoundAtlas2
{
    using System.Diagnostics;
    using SoundAtlas2.Model;

    [DebuggerDisplay("Node {NewsFeedItem.Artist.Name}")]
    public class NewReleaseHierarchyNode : IHierarchyNode
    {
        #region Properties
        public NewReleaseItem NewReleaseItem
        {
            get;
            private set;
        }
        #endregion

        public NewReleaseHierarchyNode(NewReleaseItem newReleaseItem, AtlasHierarchy hierarchy)
            : base(hierarchy, null, 0)
        {
            NewReleaseItem = newReleaseItem;
        }

        public bool IsSubTreeExpanded()
        {
            return false;
        }
    }
}
