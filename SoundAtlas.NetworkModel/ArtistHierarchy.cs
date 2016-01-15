using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Spotify;
using Spotify.Model;

namespace SoundAtlas2.Model
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

        public int AddChildrenLimit
        {
            get;
            set;
        }

        public AtlasHierarchy(SpotifyClient client, IEnumerable<Artist> artists)
        {
            Client = client;
            NodeDictionary = new Dictionary<String, HierarchyNode>();
            RootNodes = new List<HierarchyNode>();
            AddChildrenLimit = 1;

            foreach (Artist artist in artists)
            {
                GenerateTree(artist);
            }
        }

        /// <summary>
        /// Adds an artist to the root hierarchy.
        /// </summary>
        /// <param name="artistName"></param>
        public void AddRootNode(Artist artist)
        {
            GenerateTree(artist);
        }

        private void GenerateTree(Artist artist)
        {
            if (NodeDictionary.ContainsKey(artist.Name)) 
                return;

            ArtistViewModel rootViewModel = new ArtistViewModel(artist);
            HierarchyNode root = new HierarchyNode(rootViewModel, null, 0);
            RootNodes.Add(root);
            rootViewModel.SetHierarchyNode(root);
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
                    HierarchyNode relatedArtistNode = new HierarchyNode(artistViewModel, root, 1);
                    root.Children.Add(relatedArtistNode);
                    artistViewModel.SetHierarchyNode(relatedArtistNode);
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
        /// <param name="subTreeNode"></param>
        /// <returns>Returns whether the sub-tree has been fully expanded in the hierarchy.</returns>
        public void GenerateSubTree(HierarchyNode subTreeNode)
        {
            ArtistGroup relatedArtistList = Client.GetRelatedArtists(subTreeNode.ArtistViewModel.Artist.ID);

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
                    HierarchyNode relatedArtistNode = new HierarchyNode(artistViewModel, subTreeNode, subTreeNode.Level + 1);
                    subTreeNode.Children.Add(relatedArtistNode);
                    artistViewModel.SetHierarchyNode(relatedArtistNode);
                    NodeDictionary.Add(relatedArtist.Name, relatedArtistNode);

                    nodesAdded++;
                }
                else
                {
                    return;
                }
            }
        }

        public bool IsSubTreeExpanded(HierarchyNode subTreeNode)
        {
            ArtistGroup relatedArtistList = Client.GetRelatedArtists(subTreeNode.ArtistViewModel.Artist.ID);

            //First determine which artists are new; add those.
            foreach (Artist relatedArtist in relatedArtistList.Artists)
            {
                if (NodeDictionary.ContainsKey(relatedArtist.Name))
                    continue;

                if (subTreeNode.Children.Where(node => node.ArtistViewModel.Artist == relatedArtist).Any())
                {
                    return false;
                }
            }

            return true;
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
