namespace SoundAtlas2
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;

    using SoundAtlas2.Model;
    using Spotify;
    using Spotify.Model;
    using Utils;

    public class AtlasViewModel : AbstractModelBase
    {
        #region Internal Data Members
        /// <summary>
        /// The hierarchy model of nodes.
        /// </summary>
        private AtlasHierarchy _hierarchy;

        /// <summary>
        /// Determines how the node hierarchy is displayed.
        /// </summary>
        private AtlasViewMode _viewMode;

        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NetworkViewModel _network;

        ///
        /// The current scale at which the content is being viewed.
        /// 
        private double contentScale = 1;

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private double contentOffsetX = 0;

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private double contentOffsetY = 0;

        ///
        /// The width of the content (in content coordinates).
        /// 
        private double contentWidth = 0;

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        private double contentHeight = 0;

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        private double contentViewportWidth = 0;

        ///
        /// The height of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        private double contentViewportHeight = 0;

        /// <summary>
        /// Used to update the extents of the Atlas control based on the internal content.
        /// </summary>
        private Point NetworkExtents;

        #endregion Internal Data Members

        #region Constructors
        public AtlasViewModel()
        {

        }
        #endregion
        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NetworkViewModel Network
        {
            get
            {
                return _network;
            }
            set
            {
                _network = value;

                OnPropertyChanged("Network");
            }
        }

        ///
        /// The current scale at which the content is being viewed.
        /// 
        public double ContentScale
        {
            get
            {
                return contentScale;
            }
            set
            {
                contentScale = value;

                OnPropertyChanged("ContentScale");
            }
        }

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public double ContentOffsetX
        {
            get
            {
                return contentOffsetX;
            }
            set
            {
                contentOffsetX = value;

                OnPropertyChanged("ContentOffsetX");
            }
        }

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public double ContentOffsetY
        {
            get
            {
                return contentOffsetY;
            }
            set
            {
                contentOffsetY = value;

                OnPropertyChanged("ContentOffsetY");
            }
        }

        ///
        /// The width of the content (in content coordinates).
        /// 
        public double ContentWidth
        {
            get
            {
                return contentWidth;
            }
            set
            {
                contentWidth = value;

                OnPropertyChanged("ContentWidth");
            }
        }

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        public double ContentHeight
        {
            get
            {
                return contentHeight;
            }
            set
            {
                contentHeight = value;

                OnPropertyChanged("ContentHeight");
            }
        }

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        public double ContentViewportWidth
        {
            get
            {
                return contentViewportWidth;
            }
            set
            {
                contentViewportWidth = value;

                OnPropertyChanged("ContentViewportWidth");
            }
        }

        ///
        /// The heigth of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        public double ContentViewportHeight
        {
            get
            {
                return contentViewportHeight;
            }
            set
            {
                contentViewportHeight = value;

                OnPropertyChanged("ContentViewportHeight");
            }
        }


        private bool _isVisible;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        
        /// <summary>
        /// Called when the user has started to drag out a connector, thus creating a new connection.
        /// </summary>
        public ConnectionViewModel ConnectionDragStarted(ConnectorViewModel draggedOutConnector, Point curDragPoint)
        {
            //
            // Create a new connection to add to the view-model.
            //
            var connection = new ConnectionViewModel();

            if (draggedOutConnector.Type == ConnectorType.Output)
            {
                //
                // The user is dragging out a source connector (an output) and will connect it to a destination connector (an input).
                //
                connection.SourceConnector = draggedOutConnector;
                connection.DestConnectorHotspot = curDragPoint;
            }
            else
            {
                //
                // The user is dragging out a destination connector (an input) and will connect it to a source connector (an output).
                //
                connection.DestConnector = draggedOutConnector;
                connection.SourceConnectorHotspot = curDragPoint;
            }

            //
            // Add the new connection to the view-model.
            //
            this.Network.Connections.Add(connection);

            return connection;
        }

        /// <summary>
        /// Called to query the application for feedback while the user is dragging the connection.
        /// </summary>
        public void QueryConnnectionFeedback(ConnectorViewModel draggedOutConnector, ConnectorViewModel draggedOverConnector, out object feedbackIndicator, out bool connectionOk)
        {
            if (draggedOutConnector == draggedOverConnector)
            {
                //
                // Can't connect to self!
                // Provide feedback to indicate that this connection is not valid!
                //
                feedbackIndicator = new ConnectionBadIndicator();
                connectionOk = false;
            }
            else
            {
                var sourceConnector = draggedOutConnector;
                var destConnector = draggedOverConnector;

                //
                // Only allow connections from output connector to input connector (ie each
                // connector must have a different type).
                // Also only allocation from one node to another, never one node back to the same node.
                //
                connectionOk = sourceConnector.ParentNode != destConnector.ParentNode &&
                                 sourceConnector.Type != destConnector.Type;

                if (connectionOk)
                {
                    // 
                    // Yay, this is a valid connection!
                    // Provide feedback to indicate that this connection is ok!
                    //
                    feedbackIndicator = new ConnectionOkIndicator();
                }
                else
                {
                    //
                    // Connectors with the same connector type (eg input & input, or output & output)
                    // can't be connected.
                    // Only connectors with separate connector type (eg input & output).
                    // Provide feedback to indicate that this connection is not valid!
                    //
                    feedbackIndicator = new ConnectionBadIndicator();
                }
            }
        }

        /// <summary>
        /// Called as the user continues to drag the connection.
        /// </summary>
        public void ConnectionDragging(Point curDragPoint, ConnectionViewModel connection)
        {
            if (connection.DestConnector == null)
            {
                connection.DestConnectorHotspot = curDragPoint;
            }
            else
            {
                connection.SourceConnectorHotspot = curDragPoint;
            }
        }

        /// <summary>
        /// Called when the user has finished dragging out the new connection.
        /// </summary>
        public void ConnectionDragCompleted(ConnectionViewModel newConnection, ConnectorViewModel connectorDraggedOut, ConnectorViewModel connectorDraggedOver)
        {
            if (connectorDraggedOver == null)
            {
                //
                // The connection was unsuccessful.
                // Maybe the user dragged it out and dropped it in empty space.
                //
                this.Network.Connections.Remove(newConnection);
                return;
            }

            //
            // Only allow connections from output connector to input connector (ie each
            // connector must have a different type).
            // Also only allocation from one node to another, never one node back to the same node.
            //
            bool connectionOk = connectorDraggedOut.ParentNode != connectorDraggedOver.ParentNode &&
                                connectorDraggedOut.Type != connectorDraggedOver.Type;

            if (!connectionOk)
            {
                //
                // Connections between connectors that have the same type,
                // eg input -> input or output -> output, are not allowed,
                // Remove the connection.
                //
                this.Network.Connections.Remove(newConnection);
                return;
            }

            //
            // The user has dragged the connection on top of another valid connector.
            //

            //
            // Remove any existing connection between the same two connectors.
            //
            var existingConnection = FindConnection(connectorDraggedOut, connectorDraggedOver);
            if (existingConnection != null)
            {
                this.Network.Connections.Remove(existingConnection);
            }

            //
            // Finalize the connection by attaching it to the connector
            // that the user dragged the mouse over.
            //
            if (newConnection.DestConnector == null)
            {
                newConnection.DestConnector = connectorDraggedOver;
            }
            else
            {
                newConnection.SourceConnector = connectorDraggedOver;
            }
        }

        /// <summary>
        /// Retrieve a connection between the two connectors.
        /// Returns null if there is no connection between the connectors.
        /// </summary>
        public ConnectionViewModel FindConnection(ConnectorViewModel connector1, ConnectorViewModel connector2)
        {
            Trace.Assert(connector1.Type != connector2.Type);

            //
            // Figure out which one is the source connector and which one is the
            // destination connector based on their connector types.
            //
            var sourceConnector = connector1.Type == ConnectorType.Output ? connector1 : connector2;
            var destConnector = connector1.Type == ConnectorType.Output ? connector2 : connector1;

            //
            // Now we can just iterate attached connections of the source
            // and see if it each one is attached to the destination connector.
            //
            return sourceConnector.AttachedConnections.FirstOrDefault(connection => connection.DestConnector == destConnector);
        }

        /// <summary>
        /// Delete the currently selected nodes from the view-model.
        /// </summary>
        public void DeleteSelectedNodes()
        {
            // Take a copy of the selected nodes list so we can delete nodes while iterating.
            var nodesCopy = this.Network.Nodes.ToArray();
            foreach (var node in nodesCopy)
            {
                if (node.IsSelected)
                {
                    DeleteNode(node);
                }
            }
        }

        /// <summary>
        /// Delete the node from the view-model.
        /// Also deletes any connections to or from the node.
        /// </summary>
        public void DeleteNode(NetworkNodeViewModel node)
        {
            //
            // Remove all connections attached to the node.
            //
            this.Network.Connections.RemoveRange(node.AttachedConnections);

            //
            // Remove the node from the network.
            //
            this.Network.Nodes.Remove(node);
        }

        /// <summary>
        /// Create a graph node and add it to the view-model.
        /// </summary>
        public NetworkNodeViewModel CreateArtistGraphNode(IHierarchyNode hierarchyNode, Point nodeLocation) {

            Debug.Assert(hierarchyNode is ArtistHierarchyNode);

            ArtistViewModel viewModel = ((ArtistHierarchyNode)hierarchyNode).ArtistViewModel;
            ArtistNetworkNodeViewModel node = new ArtistNetworkNodeViewModel(viewModel, _hierarchy, ((ArtistHierarchyNode)hierarchyNode), PlaylistService.GetCurrentPlaylist());

            // Initialize any style modifiers.
            node.X = nodeLocation.X;
            node.Y = nodeLocation.Y;
            node.InputConnectors.Add(new ConnectorViewModel("", ConnectorType.Input));
            node.OutputConnectors.Add(new ConnectorViewModel("", ConnectorType.Output));

            hierarchyNode.GraphNodeViewModel = node;
            
            // Add the node to the view-model.
            //
            this.Network.Nodes.Add(node);

            return node;
        }


        /// <summary>
        /// Create a graph node and add it to the view-model.
        /// </summary>
        public NetworkNodeViewModel CreateNewReleasesGraphNode(IHierarchyNode hierarchyNode, Point nodeLocation)
        {
            Debug.Assert(hierarchyNode is NewReleaseHierarchyNode);

            NewReleaseItem newReleaseItem = ((NewReleaseHierarchyNode)hierarchyNode).NewReleaseItem;
            NewReleaseNetworkNodeViewModel node = new NewReleaseNetworkNodeViewModel(newReleaseItem);

            // Initialize any style modifiers.
            node.X = nodeLocation.X;
            node.Y = nodeLocation.Y;
            node.InputConnectors.Add(new ConnectorViewModel("", ConnectorType.Input));
            node.OutputConnectors.Add(new ConnectorViewModel("", ConnectorType.Output));

            hierarchyNode.GraphNodeViewModel = node;

            // Add the node to the view-model.
            //
            this.Network.Nodes.Add(node);

            return node;
        }

        /// <summary>
        /// Utility method to delete a connection from the view-model.
        /// </summary>
        public void DeleteConnection(ConnectionViewModel connection)
        {
            this.Network.Connections.Remove(connection);
        }

        /// <summary>
        /// Populates the sub-tree for all selected nodes.
        /// </summary>
        public bool HandleSelectedNodes()
        {
            bool nodeSelected = false;
            foreach(NetworkNodeViewModel node in this.Network.Nodes)
            {
                if (node.IsSelected)
                {
                    node.OnSelected();
                    nodeSelected = true;
                }
            }

            return nodeSelected;
        }

        public void SelectArtistNodes(IEnumerable<Artist> artists)
        {
            foreach (ArtistNetworkNodeViewModel node in this.Network.Nodes.OfType<ArtistNetworkNodeViewModel>())
            {
                if (artists.Any(artist => artist.ID == node.ArtistViewModel.Artist.ID))
                {
                    node.IsSelected = true;
                }
                else
                {
                    node.IsSelected = false;
                }
            }
        }

        #region Generation Methods
        public void CreatePlaylistHierarchy(IReadOnlyCollection<Artist> targetArtists, AtlasViewOptions options)
        {
            _viewMode = AtlasViewMode.PlaylistView;
            _hierarchy = new ArtistAtlasHierarchy(SpotifyClientService.Client, targetArtists, options);
        }

        public void CreateFollowedArtistHierarchy(IReadOnlyCollection<Artist> targetArtists, AtlasViewOptions options)
        {
            _viewMode = AtlasViewMode.FollowedArtistView;
            _hierarchy = new ArtistAtlasHierarchy(SpotifyClientService.Client, targetArtists, options);
        }

        public void CreateNewReleaseHierarchy(IReadOnlyCollection<NewReleaseItem> newReleases, AtlasViewOptions options)
        {
            _viewMode = AtlasViewMode.NewReleasesView;
            _hierarchy = new NewReleaseAtlasHierarchy(SpotifyClientService.Client, newReleases, options);
        }

        public void AddArtistsToHierarchy(IReadOnlyCollection<Artist> targetArtists, AtlasViewOptions options)
        {
            if (_hierarchy == null)
            {
                _hierarchy = new ArtistAtlasHierarchy(SpotifyClientService.Client, targetArtists, options);
            }
            else
            {
                foreach (Artist artist in targetArtists)
                    _hierarchy.AddRootNode(artist);
            }
        }

        public void AddNewReleasesToHierarchy(IReadOnlyCollection<NewReleaseItem> newReleases, AtlasViewOptions options) {

            if (_hierarchy == null) {
                _hierarchy = new NewReleaseAtlasHierarchy(SpotifyClientService.Client, newReleases, options);
            }
            else {
                foreach (NewReleaseItem newReleaseItem in newReleases) {
                    _hierarchy.AddRootNode(newReleaseItem);
                }
            }
        }

        public void GenerateNetwork()
        {
            //Once we have the hierarchy, assemble the nodes.
            this.Network = new NetworkViewModel();

            int offset = 0;
            foreach (IHierarchyNode hierarchyNode in _hierarchy.RootNodes)
            {
                Point graphNodeLocation = new Point(0, offset);
                NetworkNodeViewModel graphNode = null;

                if (hierarchyNode is ArtistHierarchyNode) {
                    graphNode = CreateArtistGraphNode(hierarchyNode, graphNodeLocation);
                }
                else if (hierarchyNode is NewReleaseHierarchyNode) {
                    graphNode = CreateNewReleasesGraphNode(hierarchyNode, graphNodeLocation);
                }

                GenerateNodes(hierarchyNode, graphNode, new Point(1, 0));
                offset++;
            }
        }
        
        private void GenerateNodes(IHierarchyNode parentNode, NetworkNodeViewModel parentGraphNode, Point point)
        {
            int offset = 0;
            foreach (IHierarchyNode hierarchyNode in parentNode.Children)
            {
                Point graphNodeLocation = new Point(point.X, offset);

                NetworkNodeViewModel graphNode = null;

                if (hierarchyNode is ArtistHierarchyNode)
                {
                    graphNode = CreateArtistGraphNode(hierarchyNode, graphNodeLocation);
                }
                else if (hierarchyNode is NewReleaseHierarchyNode)
                {
                    graphNode = CreateNewReleasesGraphNode(hierarchyNode, graphNodeLocation);
                }

                ConnectionViewModel connection = new ConnectionViewModel();
                connection.SourceConnector = parentGraphNode.OutputConnectors[0];
                connection.DestConnector = graphNode.InputConnectors[0];

                // Add the connection to the graph.
                this.Network.Connections.Add(connection);

                //Generate any children.
                GenerateNodes(hierarchyNode, graphNode, new Point(point.X + 1, 0));

                offset++;
            }
        }

        public void ArrangeNetwork()
        {
            if (!this.Network.Nodes.Any())
                return;

            NetworkExtents = new Point(this.ContentWidth, this.ContentWidth);

            switch (_viewMode) {
                case AtlasViewMode.PlaylistView:
                case AtlasViewMode.FollowedArtistView:
                {
                    int level = 0;
                    double maxNodeLevelWidth = 0.0;
                    while (true)
                    {
                        IEnumerable<IHierarchyNode> levelNodes = _hierarchy.GetNodesAtLevel(level);

                        if (!levelNodes.Any())
                        {
                            //No nodes in the tree at this level.  Break.
                            break;
                        }

                        //Find the location of this node based on.
                        double currentNodeWidth = maxNodeLevelWidth;
                        double previousNodeHeight = 0.0;
                        Point startLocation = new Point((level * currentNodeWidth) + (level != 0 ? (level * 50.0) : 0.0), 0.0);
                        double heightPadding = 5.0;

                        foreach (IHierarchyNode levelNode in levelNodes)
                        {
                            levelNode.GraphNodeViewModel.X = startLocation.X;
                            levelNode.GraphNodeViewModel.Y = previousNodeHeight;

                            //Ensure that if the child node is not higher than its parent.
                            if (levelNode.Parent != null && levelNode.GraphNodeViewModel.Y < levelNode.Parent.GraphNodeViewModel.Y)
                            {
                                levelNode.GraphNodeViewModel.Y = levelNode.Parent.GraphNodeViewModel.Y;
                            }

                            if (levelNode.GraphNodeViewModel.Size.Width > maxNodeLevelWidth)
                            {
                                maxNodeLevelWidth = levelNode.GraphNodeViewModel.Size.Width;
                            }

                            int childrenCount = GetChildrenDepth(levelNode);
                            previousNodeHeight = levelNode.GraphNodeViewModel.Y + (childrenCount * (levelNode.GraphNodeViewModel.Size.Height + heightPadding));
                        }

                        level++;
                    }
                }
                break;
                case AtlasViewMode.NewReleasesView:
                {
                    //There should only be nodes on the root level.
                    Point startLocation = new Point(0.0f, 5.0f);
                    double widthPadding = 5.0f;
                    foreach(IHierarchyNode hierarchyNode in _hierarchy.RootNodes)
                    {
                        hierarchyNode.GraphNodeViewModel.X = startLocation.X;
                        hierarchyNode.GraphNodeViewModel.Y = startLocation.Y;

                        startLocation.X = hierarchyNode.GraphNodeViewModel.X + hierarchyNode.GraphNodeViewModel.Size.Width + widthPadding;
                    }
                }
                break;
            }

            foreach (NetworkNodeViewModel nodeViewModel in this.Network.Nodes)
            {
                NetworkExtents.X = Math.Max(NetworkExtents.X, nodeViewModel.X + nodeViewModel.Size.Width);
                NetworkExtents.Y = Math.Max(NetworkExtents.Y, nodeViewModel.Y + nodeViewModel.Size.Height);
            }
                
            this.ContentWidth = NetworkExtents.X;
            this.ContentHeight = NetworkExtents.Y;
        }

        public int GetChildrenDepth(IHierarchyNode targetNode)
        {
            if (!targetNode.Children.Any())
            {
                return 1;
            }
            else
            {
                int totalDepth = 0;
                foreach (IHierarchyNode childNode in targetNode.Children)
                {
                    totalDepth += GetChildrenDepth(childNode);
                }

                return totalDepth;
            }
        }
        
        public T FindNode<T>(string id) where T : NetworkNodeViewModel
        {
            return this.Network.Nodes.OfType<T>().Where(node => String.Equals(node.ID, id)).FirstOrDefault();
        }
        #endregion
    }
}
