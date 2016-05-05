namespace SoundAtlas2.Model
{
    using System.Collections.Generic;

    public abstract class IHierarchyNode
    {
        public NetworkNodeViewModel GraphNodeViewModel
        {
            get;
            set;
        }

        protected AtlasHierarchy Hierarchy
        {
            get;
            private set;
        }

        public List<IHierarchyNode> Children
        {
            get;
            private set;
        }

        public IHierarchyNode Parent
        {
            get;
            private set;
        }

        public int Level
        {
            get;
            private set;
        }

        public IHierarchyNode(AtlasHierarchy hierarchy, IHierarchyNode parent, int level)
        {
            Hierarchy = hierarchy;
            Children = new List<IHierarchyNode>();
            Level = level;
            GraphNodeViewModel = null;
        }
        
        public void ClearSiblings()
        {
            if (Parent != null)
            {
                Parent.Children.ForEach(child => child.Clear());
            }
        }
        
        public void Clear()
        {
            Children.ForEach(child => child.Clear());
            Children.Clear();
        }
        
        public bool HasChild(IHierarchyNode node)
        {
            if (Children.Contains(node)) return true;

            bool isChild = false;
            Children.ForEach(child =>
            {
                if (child.HasChild(node))
                {
                    isChild = true;
                }
            });

            return isChild;
        }
    }
}
