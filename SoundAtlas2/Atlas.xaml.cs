using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NetworkUI;
using SoundAtlas2.Model;
using Utils;

namespace SoundAtlas2
{
    /// <summary>
    /// Interaction logic for Atlas.xaml
    /// </summary>
    public partial class Atlas : DockPanel
    {
        public Atlas()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public AtlasViewModel ViewModel
        {
            get
            {
                return (AtlasViewModel)DataContext;
            }
        }

        #region Events
        public event RoutedEventHandler AddPopularTracks
        {
            add { AddHandler(AddPopularTracksEvent, value); }
            remove { RemoveHandler(AddPopularTracksEvent, value); }
        }
       
        public readonly RoutedEvent AddPopularTracksEvent = EventManager.RegisterRoutedEvent("AddPopularTracks", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Atlas));
        #endregion

        /// <summary>
        /// Event raised when the user has started to drag out a connection.
        /// </summary>
        private void networkControl_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
        {
            var draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            var curDragPoint = Mouse.GetPosition(networkControl);

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
        private void networkControl_QueryConnectionFeedback(object sender, QueryConnectionFeedbackEventArgs e)
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
        private void networkControl_ConnectionDragging(object sender, ConnectionDraggingEventArgs e)
        {
            Point curDragPoint = Mouse.GetPosition(networkControl);
            var connection = (ConnectionViewModel)e.Connection;
            this.ViewModel.ConnectionDragging(curDragPoint, connection);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a connection.
        /// </summary>
        private void networkControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
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
        private void CreateNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNode();
        }

        /// <summary>
        /// Event raised to delete a node.
        /// </summary>
        private void DeleteNode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var node = (NodeViewModel)e.Parameter;
            this.ViewModel.DeleteNode(node);
        }

        /// <summary>
        /// Event raised to delete a connection.
        /// </summary>
        private void DeleteConnection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var connection = (ConnectionViewModel)e.Parameter;
            this.ViewModel.DeleteConnection(connection);
        }

        /// <summary>
        /// Creates a new node in the network at the current mouse location.
        /// </summary>
        private void CreateNode()
        {
            var newNodePosition = Mouse.GetPosition(networkControl);
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

        private void networkControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void UpdateNetwork()
        {
            this.ViewModel.GenerateNetwork();
            this.UpdateLayout();
            this.ViewModel.ArrangeNetwork();
        }

        private void OnAddPopularTracksClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel viewModel = (NodeViewModel)((FrameworkElement)sender).Tag;
            
            viewModel.IsInPlaylist = true;

            RoutedEventArgs newEventArgs = new RoutedEventArgs(AddPopularTracksEvent, viewModel);
            RaiseEvent(newEventArgs);
        }

        private void OnExpandRelatedArtistsButtonClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel targetNode = (NodeViewModel)((FrameworkElement)sender).Tag;

            this.ViewModel.CreateNodeChildren(targetNode.ArtistViewModel.HierarchyNode.GraphNodeViewModel, false);
            UpdateNetwork();
        }

        private void OnExpandAllRelatedArtistsButtonClick(object sender, RoutedEventArgs e)
        {
            NodeViewModel targetNode = (NodeViewModel)((FrameworkElement)sender).Tag;

            this.ViewModel.CreateNodeChildren(targetNode.ArtistViewModel.HierarchyNode.GraphNodeViewModel, true);
            UpdateNetwork();
        }
    }
}
