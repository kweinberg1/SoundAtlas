using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Spotify;
using Spotify.Model;

namespace SoundAtlas
{
    public class ArtistHierarchy
    {
        [DebuggerDisplay("Node {ArtistViewModel.Artist.Name}")]
        public class Node 
        {
            public Node(ArtistViewModel artistViewModel, Node parent, int level)
            {
                ArtistViewModel = artistViewModel;
                Parent = parent;
                Children = new List<Node>();
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

            public bool HasChild(Node node)
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
            public Node Parent;
            public List<Node> Children;
            public int Level; 
        }

        public List<Node> RootNodes;
        private Dictionary<String, Node> NodeDictionary;
        private SpotifyClient Client;

        public ArtistHierarchy(SpotifyClient client, IEnumerable<String> artistNames)
        {
            Client = client;
            NodeDictionary = new Dictionary<String, Node>();
            RootNodes = new List<Node>();

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

            ArtistList rootArtistList = Client.SearchArtists(artistName);

            //TODO: Using the first one for now.  Since there could be multiple roots, we'll need to handle this.
            Artist rootArtist = rootArtistList.ArtistGroup.Items[0];
            ArtistViewModel rootViewModel = new ArtistViewModel(rootArtist);
            Node root = new Node(rootViewModel, null, 0);
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
                Node relatedArtistNode = new Node(artistViewModel, root, 1);
                root.Children.Add(relatedArtistNode);
                artistViewModel.SetHierarchyNode(relatedArtistNode);
                NodeDictionary.Add(relatedArtist.Name, relatedArtistNode);
            }
        }

        public void GenerateSubTree(Node subTreeNode)
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
                Node relatedArtistNode = new Node(artistViewModel, subTreeNode, subTreeNode.Level+1);
                subTreeNode.Children.Add(relatedArtistNode);
                artistViewModel.SetHierarchyNode(relatedArtistNode);
                NodeDictionary.Add(relatedArtist.Name, relatedArtistNode);
            }
        }

        public List<Node> GetNodesAtLevel(int level)
        {
            if (level == 0)
                return RootNodes;

            List<Node> levelNodeList = new List<Node>();
            foreach (Node rootNode in RootNodes)
            {
                GetNodesAtLevel(level, rootNode, ref levelNodeList);
            }
            return levelNodeList;
        }

        public void GetNodesAtLevel(int level, Node targetNode, ref List<Node> levelNodeList)
        {
            foreach (Node childNode in targetNode.Children)
            {
                if (childNode.Level == level)
                    levelNodeList.Add(childNode);
                else
                    GetNodesAtLevel(level, childNode, ref levelNodeList);
            }
        }
    }
}
