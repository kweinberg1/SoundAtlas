namespace SoundAtlas2.Model
{
    using System;
    using System.Collections.Generic;

    using System.Linq;
    using Spotify;
    using Spotify.Model;

    public abstract class AtlasHierarchy
    {
        public List<IHierarchyNode> RootNodes;
        protected Dictionary<String, IHierarchyNode> NodeDictionary;
        protected SpotifyClient Client;

        public int AddChildrenLimit
        {
            get;
            set;
        }

        public AtlasHierarchy(SpotifyClient client, AtlasViewOptions viewOptions)
        {
            Client = client;
            NodeDictionary = new Dictionary<String, IHierarchyNode>();
            RootNodes = new List<IHierarchyNode>();
            AddChildrenLimit = viewOptions.AddChildrenCount;
        }

        public abstract void AddRootNode(object obj);
        protected abstract void GenerateTree(object obj);

        public abstract void GenerateSubTree(IHierarchyNode node);

        public IEnumerable<IHierarchyNode> GetNodesAtLevel(int level)
        {
            if (level == 0)
                return RootNodes;

            List<IHierarchyNode> levelNodeList = new List<IHierarchyNode>();
            foreach (IHierarchyNode rootNode in RootNodes)
            {
                GetNodesAtLevel(level, rootNode, ref levelNodeList);
            }
            return levelNodeList;
        }

        public void GetNodesAtLevel(int level, IHierarchyNode targetNode, ref List<IHierarchyNode> levelNodeList)
        {
            foreach (IHierarchyNode childNode in targetNode.Children)
            {
                if (childNode.Level == level)
                    levelNodeList.Add(childNode);
                else
                    GetNodesAtLevel(level, childNode, ref levelNodeList);
            }
        }

        public bool NodeExists(string key)
        {
            return NodeDictionary.ContainsKey(key);
        }
    }
}
