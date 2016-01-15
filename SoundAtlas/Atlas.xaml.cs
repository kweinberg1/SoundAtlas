using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

using Spotify.Model;

namespace SoundAtlas
{
    /// <summary>
    /// Interaction logic for Atlas.xaml
    /// </summary>
    public partial class Atlas : ListBox
    {
        #region Dependency Properties
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(int), typeof(ArtistPanel));
        #endregion

        #region Properties
        private readonly float ZoomScaleIncrement = 0.10f;
        private float MinZoomSetting = 0.10f;
        private float MaxZoomSetting = 2.0f;

        private Point ClickPosition;
        private AtlasViewModel _viewModel;
        private ArtistViewModel _selectedArtist;
        private PlaylistViewModel _playlistViewModel;
        #endregion

        #region Constructors
        public Atlas()
        {
            InitializeComponent();

            _viewModel = new AtlasViewModel();
            this.DataContext = _viewModel;
        }

        public void GenerateMap(IEnumerable<String> inputArtists, PlaylistViewModel playlistViewModel)
        {
            AtlasViewModel atlasViewModel = (AtlasViewModel)this.DataContext;

            _playlistViewModel = playlistViewModel;

            atlasViewModel.Generate(inputArtists);

            if (atlasViewModel.Artists.Any())
            {
                this.AtlasListBox.SelectedItem = atlasViewModel.Artists[0];
            }

            foreach(ArtistViewModel artistViewModel in atlasViewModel.Artists)
            {
                ArtistPanel artistPanel = new ArtistPanel();
                artistPanel.DataContext = artistViewModel;
                
                this.AddChild(artistPanel);
            }

            //Force all controls to be evaluated.
            this.AtlasListBox.UpdateLayout();

            //Create all edges.
            List<EdgeViewModel> edges = new List<EdgeViewModel>();

            IEnumerable<ArtistPanel> artistPanels = FindVisualChildren<ArtistPanel>(this.AtlasListBox);
            foreach (ArtistPanel artistPanel in artistPanels)
            {
                ArtistViewModel artistViewModel = (ArtistViewModel)artistPanel.DataContext;

                foreach (ArtistHierarchy.Node child in artistViewModel.HierarchyNode.Children)
                {
                    ArtistPanel targetArtistPanel = artistPanels.Where(panel => ((ArtistViewModel)panel.DataContext) == child.ArtistViewModel).First();

                    Edge newEdge = new Edge();
                    EdgeViewModel edgeViewModel = new EdgeViewModel(artistPanel, targetArtistPanel, this.AtlasListBox);
                    newEdge.DataContext = edgeViewModel;
                    
                    edges.Add(edgeViewModel);

                    this.AddChild(newEdge);
                }
            }

            atlasViewModel.Edges = edges;
        }
        #endregion

        #region Event Handlers
        /*protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_viewModel != null && _viewModel.RootNodes != null)
            {
                IEnumerable<ArtistPanel> artistPanels = FindVisualChildren<ArtistPanel>(this.AtlasListBox);

                foreach (ArtistPanel artistPanel in artistPanels)
                {
                    DrawEdges(dc, artistPanel, artistPanels);
                }
            }
        }*/

        /*private void DrawEdges(DrawingContext dc, ArtistPanel artistPanel, IEnumerable<ArtistPanel> artistPanels)
        {
            ArtistViewModel artistViewModel = (ArtistViewModel)artistPanel.DataContext;

            Point startPoint = new Point(artistViewModel.X, artistViewModel.Y); //TODO: This is really inefficient.  This should be fixed.

            foreach (ArtistHierarchy.Node childNode in artistViewModel.HierarchyNode.Children)
            {
                Point endPoint = new Point(childNode.ArtistViewModel.X, childNode.ArtistViewModel.Y);

                AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);

                //Find the artist panel
                //TODO:  This is extremely slow. :(
                ArtistPanel childArtistPanel = artistPanels.Where(panel =>
                    {
                        ArtistViewModel viewModel = (ArtistViewModel)panel.DataContext;
                        return (viewModel.HierarchyNode == childNode);
                    }).FirstOrDefault();


                layer.Add(new EdgeViewModel(artistPanel, childArtistPanel, this));
            }
        }*/

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            ScrollViewer viewer = VisualTreeHelperExtensions.GetFirstVisualChild<ScrollViewer>(listBox);
            ItemsPresenter itemsPresenter = VisualTreeHelperExtensions.GetFirstVisualChild<ItemsPresenter>(listBox);
            ScrollableCanvas scrollableCanvas = VisualTreeHelperExtensions.GetFirstVisualChild<ScrollableCanvas>(itemsPresenter);

            Point mouseAtImage = e.GetPosition(viewer);
            Point mouseAtScrollViewer = e.GetPosition(listBox);

            ScaleTransform scaleTransform = scrollableCanvas.LayoutTransform as ScaleTransform;
            if (scaleTransform == null)
            {
                scaleTransform = new ScaleTransform();
                scrollableCanvas.LayoutTransform = scaleTransform;
            }

            if (e.Delta > 0)
            {
                scaleTransform.ScaleX = scaleTransform.ScaleY = scaleTransform.ScaleX + ZoomScaleIncrement;
                if (scaleTransform.ScaleX > MaxZoomSetting) 
                    scaleTransform.ScaleX = scaleTransform.ScaleY = MaxZoomSetting;
            }
            else
            {
                scaleTransform.ScaleX = scaleTransform.ScaleY = scaleTransform.ScaleX - ZoomScaleIncrement;
                if (scaleTransform.ScaleX < MinZoomSetting)
                    scaleTransform.ScaleX = scaleTransform.ScaleY = MinZoomSetting;
            }

            OnSelectionChanged(this.AtlasListBox, null);

            e.Handled = true;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox userControl = sender as ListBox;
            ClickPosition = e.GetPosition(userControl);
       
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(userControl, ClickPosition);
            if (hitTestResult.VisualHit != null)
            {
                DependencyObject hitElement = hitTestResult.VisualHit;
                DependencyObject artistElement = FindRegisteredModel(hitElement);
                if (artistElement is ArtistPanel)
                {
                    ArtistPanel artistPanel = (ArtistPanel)artistElement;
                    //artistPanel.CaptureMouse();
                }  
            }
            else
            {
                userControl.CaptureMouse();
            }
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var userControl = sender as ListBox;
            userControl.ReleaseMouseCapture();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var userControl = sender as ListBox;

            if (!userControl.IsMouseCaptured) 
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point positionRelative = e.GetPosition(userControl);

                //Scroll the canvas itself if the left mouse button is pressed.
                double offsetX = (ClickPosition.X - positionRelative.X);
                double offsetY = (ClickPosition.Y - positionRelative.Y);

                Border border = (Border)VisualTreeHelper.GetChild(userControl, 0);
                ScrollViewer viewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);

                viewer.ScrollToHorizontalOffset(viewer.HorizontalOffset + offsetX);
                viewer.ScrollToVerticalOffset(viewer.VerticalOffset + offsetY);

                ClickPosition = positionRelative;
            }

            this.InvalidateVisual();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox atlasListBox = (ListBox) sender;

            //Set the scroll offsets.
            Border border = (Border)VisualTreeHelper.GetChild(atlasListBox, 0);
            ScrollViewer viewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);

            if (atlasListBox.SelectedItem is ArtistPanel)
            {
                ArtistPanel panel = (ArtistPanel)atlasListBox.SelectedItem;
                ArtistViewModel selectedViewModel = (ArtistViewModel)panel.DataContext;

                if (selectedViewModel != null && _selectedArtist != selectedViewModel)
                {
                    _selectedArtist = selectedViewModel;

                    atlasListBox.ScrollToCenterOfView(_selectedArtist);

                    if (!selectedViewModel.HierarchyNode.Children.Any())
                    {
                        _viewModel.GenerateChildren(selectedViewModel);
                    }
                }

                this.InvalidateVisual();
            }
        }
        #endregion

        #region Helper Functions
        private DependencyObject FindRegisteredModel(DependencyObject hitElement)
        {
            DependencyObject currentObject = hitElement;
            while (currentObject != null)
            {
                if (currentObject is ArtistPanel)
                    return currentObject;

                currentObject = LogicalTreeHelper.GetParent(currentObject);
            }

            return null;
        }

        /// <summary>
        /// Gathers all children in the visual tree within the below UIElement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject obj)
            where T : DependencyObject
        {
            List<T> children = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    children.Add((T)child);
                else
                {
                    IEnumerable<T> childOfChild = FindVisualChildren<T>(child);
                    if (childOfChild != null)
                        children.AddRange(childOfChild);
                }
            }

            return children;
        }
        #endregion
    }
}
