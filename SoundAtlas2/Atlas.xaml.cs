namespace SoundAtlas2
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using NetworkUI;
    using SoundAtlas2.Model;
    
    /// <summary>
    /// Interaction logic for Atlas.xaml
    /// </summary>
    public partial class Atlas : DockPanel
    {
        #region Constructors
        public Atlas()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public AtlasViewModel ViewModel => (AtlasViewModel) DataContext;
        #endregion

        #region Events
        public event RoutedEventHandler AddTracks
        {
            add { AddHandler(AddTracksEvent, value); }
            remove { RemoveHandler(AddTracksEvent, value); }
        }
        public readonly RoutedEvent AddTracksEvent = EventManager.RegisterRoutedEvent("AddTracks", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Atlas));

        public event RoutedEventHandler FollowArtist
        {
            add { AddHandler(FollowArtistEvent, value); }
            remove { RemoveHandler(FollowArtistEvent, value); }
        }
        public readonly RoutedEvent FollowArtistEvent = EventManager.RegisterRoutedEvent("FollowArtist", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Atlas));

        public event RoutedEventHandler UnfollowArtist
        {
            add { AddHandler(UnfollowArtistEvent, value); }
            remove { RemoveHandler(UnfollowArtistEvent, value); }
        }
        public readonly RoutedEvent UnfollowArtistEvent = EventManager.RegisterRoutedEvent("UnfollowArtist", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Atlas));

        public event RoutedEventHandler RegenerateNetwork
        {
            add { AddHandler(RegenerateNetworkEvent, value); }
            remove { RemoveHandler(RegenerateNetworkEvent, value); }
        }
        public readonly RoutedEvent RegenerateNetworkEvent = EventManager.RegisterRoutedEvent("RegenerateNetwork", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Atlas));

        #endregion

        #region Event Handlers
        /// <summary>
        /// Event raised when the user has started to drag out a connection.
        /// </summary>
        private void NetworkControl_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e) {
            NetworkView networkView = sender as NetworkView;
            var draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            var curDragPoint = Mouse.GetPosition(networkView);

            //
            // Delegate the real work to the view model.
            //
            var connection = this.ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);

            //
            // Must return the view-model object that represents the connection via the event args.
            // This is so that NetworkView can keep track of the object while it is being dragged.
            //
            e.Connection = connection;
        }

        /// <summary>
        /// Event raised, to query for feedback, while the user is dragging a connection.
        /// </summary>
        private void NetworkControl_QueryConnectionFeedback(object sender, QueryConnectionFeedbackEventArgs e)
        {
            var draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            var draggedOverConnector = (ConnectorViewModel)e.DraggedOverConnector;
            object feedbackIndicator = null;
            bool connectionOk = true;

            this.ViewModel.QueryConnnectionFeedback(draggedOutConnector, draggedOverConnector, out feedbackIndicator, out connectionOk);

            //
            // Return the feedback object to NetworkView.
            // The object combined with the data-template for it will be used to create a 'feedback icon' to
            // display (in an adorner) to the user.
            //
            e.FeedbackIndicator = feedbackIndicator;

            //
            // Let NetworkView know if the connection is ok or not ok.
            //
            e.ConnectionOk = connectionOk;
        }

        /// <summary>
        /// Event raised while the user is dragging a connection.
        /// </summary>
        private void NetworkControl_ConnectionDragging(object sender, ConnectionDraggingEventArgs e)
        {
            NetworkView networkView = sender as NetworkView;
            Point curDragPoint = Mouse.GetPosition(networkView);
            var connection = (ConnectionViewModel)e.Connection;
            this.ViewModel.ConnectionDragging(curDragPoint, connection);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a connection.
        /// </summary>
        private void NetworkControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
        {
            var connectorDraggedOut = (ConnectorViewModel)e.ConnectorDraggedOut;
            var connectorDraggedOver = (ConnectorViewModel)e.ConnectorDraggedOver;
            var newConnection = (ConnectionViewModel)e.Connection;
            this.ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);
        }

        /// <summary>
        /// Event raised to delete the selected node.
        /// </summary>
        private void DeleteSelectedNodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.DeleteSelectedNodes();
        }

        /// <summary>
        /// Event raised to create a new node.
        /// </summary>
        private void CreateNode_Executed(object sender, ExecutedRoutedEventArgs e) {
            NetworkView networkView = sender as NetworkView;
            CreateNode(networkView);
        }

        /// <summary>
        /// Creates a new node in the network at the current mouse location.
        /// </summary>
        private void CreateNode(NetworkView networkView)
        {
            var newNodePosition = Mouse.GetPosition(networkView);
            this.ViewModel.CreateNode("New Node!", newNodePosition, true);
        }

        /// <summary>
        /// Event raised when the size of a node has changed.
        /// </summary>
        private void Node_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //
            // The size of a node, as determined in the UI by the node's data-template,
            // has changed.  Push the size of the node through to the view-model.
            //
            var element = (FrameworkElement)sender;
            var node = (NodeViewModel)element.DataContext;
            node.Size = new Size(element.ActualWidth, element.ActualHeight);
        }

        /// <summary>
        /// Event raised to delete the selected node.
        /// </summary>
        private void GenerateNetwork_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(RegenerateNetworkEvent, sender);
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        /// Updates a network with an entirely new view.
        /// </summary>
        public void UpdateNetwork()
        {
            this.ViewModel.GenerateNetwork();
            this.UpdateLayout();
            this.ViewModel.ArrangeNetwork();
        }

        /// <summary>
        /// Event handler for adding tracks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddTracksClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel viewModel = (NodeViewModel)((FrameworkElement)sender).Tag;
            
            viewModel.IsInPlaylist = true;

            RoutedEventArgs newEventArgs = new RoutedEventArgs(AddTracksEvent, viewModel);
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        /// Event handler for following an artist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFollowArtistClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel viewModel = (NodeViewModel)((FrameworkElement)sender).Tag;

            viewModel.IsHighlighted = true;

            RoutedEventArgs newEventArgs = new RoutedEventArgs(FollowArtistEvent, viewModel);
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        /// Event handler for unfollowing an artist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnfollowArtistClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel viewModel = (NodeViewModel)((FrameworkElement)sender).Tag;

            viewModel.IsHighlighted = false;

            RoutedEventArgs newEventArgs = new RoutedEventArgs(UnfollowArtistEvent, viewModel);
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        /// Event handler to expand an artist's node with another related artist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExpandRelatedArtistsButtonClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel targetNode = (NodeViewModel)((FrameworkElement)sender).Tag;

            this.ViewModel.CreateNodeChildren(targetNode.ArtistViewModel.HierarchyNode.GraphNodeViewModel, AtlasCreateChildSetting.CreateOneChild);
            UpdateNetwork();
        }

        /// <summary>
        /// Event handler to expand an artist's node with all other related artists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExpandAllRelatedArtistsButtonClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel targetNode = (NodeViewModel)((FrameworkElement)sender).Tag;

            this.ViewModel.CreateNodeChildren(targetNode.ArtistViewModel.HierarchyNode.GraphNodeViewModel, AtlasCreateChildSetting.CreateAllChildren);
            UpdateNetwork();
        }
        #endregion
    }
}
