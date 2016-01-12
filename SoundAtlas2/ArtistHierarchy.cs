using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using SoundAtlas2.NetworkModel;
using Spotify;
using Spotify.Model;

namespace SoundAtlas2
{
    public class AtlasHierarchy
    {
        [DebuggerDisplay("Node {ArtistViewModel.Artist.Name}")]
        public class HierarchyNode
        {
            public HierarchyNode(ArtistViewModel artistViewModel, HierarchyNode parent, int level)
            {
                ArtistViewModel = artistViewModel;
                GraphNodeViewModel = null;
                Parent = parent;
                Children = new List<HierarchyNode>();
                Level = level;
            }

            public void Clear()
            {
                Children.ForEach(child => child.Clear());
                Children.Clear();
            }

            public void ClearSiblings()
            {
                if (Parent != null)
                {
                    Parent.Children.ForEach(child => child.Clear());
                }
            }

            public bool HasChild(HierarchyNode node)
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

            public ArtistViewModel ArtistViewModel;
            public NodeViewModel GraphNodeViewModel;
            public HierarchyNode Parent;
            public List<HierarchyNode> Children;
            public int Level; 
        }

        public List<HierarchyNode> RootNodes;
        private Dictionary<String, HierarchyNode> NodeDictionary;
        private SpotifyClient Client;

        public AtlasHierarchy(SpotifyClient client, IEnumerable<String> artistNames)
        {
            Client = client;
            NodeDictionary = new Dictionary<String, HierarchyNode>();
            RootNodes = new List<HierarchyNode>();

            foreach (String artistName in artistNames)
            {
                GenerateTree(artistName);
            }
        }

        private void GenerateTree(String artistName)
        {
            //Going to separate artists based on the genres used to describe them.  
            //Assume that the first artist is what we're pivoting on.

            if (NodeDictionary.ContainsKey(artistName)) 
                return;

            ArtistList rootArtistList = Client.GetArtist(artistName);

            //TODO: Using the first one for now.  Since there could be multiple roots, we'll need to handle this.
            Artist rootArtist = rootArtistList.Artists.Items[0];
            ArtistViewModel rootViewModel = new ArtistViewModel(rootArtist);
            HierarchyNode root = new HierarchyNode(rootViewModel, null, 0);
            RootNodes.Add(root);
            rootViewModel.SetHierarchyNode(root);
            NodeDictionary.Add(rootArtist.Name, root);

            ArtistGroup relatedArtistList = Client.GetRelatedArtists(rootArtist.ID);

            //First determine which artists are new; add those.
            List<Artist> newArtists = new List<Artist>();
            foreach (Artist relatedArtist in relatedArtistList.Artists)
            {
                if (NodeDictionary.ContainsKey(relatedArtist.Name))
                    continue;

                newArtists.Add(relatedArtist);

                ArtistViewModel artistViewModel = new ArtistViewModel(relatedArtist);
                HierarchyNode relatedArtistNode = new HierarchyNode(artistViewModel, root, 1);
                root.Children.Add(relatedArtistNode);
                artistViewModel.SetHierarchyNode(relatedArtistNode);
                NodeDictionary.Add(relatedArtist.Name, relatedArtistNode);
            }
        }

        public void GenerateSubTree(HierarchyNode subTreeNode)
        {
            ArtistGroup relatedArtistList = Client.GetRelatedArtists(subTreeNode.ArtistViewModel.Artist.ID);

            //First determine which artists are new; add those.
            List<Artist> newArtists = new List<Artist>();
            foreach (Artist relatedArtist in relatedArtistList.Artists)
            {
                if (NodeDictionary.ContainsKey(relatedArtist.Name))
                    continue;

                newArtists.Add(relatedArtist);

                ArtistViewModel artistViewModel = new ArtistViewModel(relatedArtist);
                HierarchyNode relatedArtistNode = new HierarchyNode(artistViewModel, subTreeNode, subTreeNode.Level + 1);
                subTreeNode.Children.Add(relatedArtistNode);
                artistViewModel.SetHierarchyNode(relatedArtistNode);
                NodeDictionary.Add(relatedArtist.Name, relatedArtistNode);
            }
        }

        public IEnumerable<HierarchyNode> GetNodesAtLevel(int level)
        {
            if (level == 0)
                return RootNodes;

            List<HierarchyNode> levelNodeList = new List<HierarchyNode>();
            foreach (HierarchyNode rootNode in RootNodes)
            {
                GetNodesAtLevel(level, rootNode, ref levelNodeList);
            }
            return levelNodeList;
        }

        public void GetNodesAtLevel(int level, HierarchyNode targetNode, ref List<HierarchyNode> levelNodeList)
        {
            foreach (HierarchyNode childNode in targetNode.Children)
            {
                if (childNode.Level == level)
                    levelNodeList.Add(childNode);
                else
                    GetNodesAtLevel(level, childNode, ref levelNodeList);
            }
        }
    }
}
