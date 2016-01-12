using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Spotify;
using Spotify.Model;

namespace SoundAtlas
{
    internal class AtlasViewModel : INotifyPropertyChanged
    {
        #region Members
        private ArtistHierarchy _hierarchy;
        private Size _desiredArtistPanelSize;
        #endregion

        #region Properties
        public String Title
        {
            get;
            private set;
        }

        private IEnumerable<EdgeViewModel> _edges;
        public IEnumerable<EdgeViewModel> Edges
        {
            get { return _edges; }

            set
            {
                if (_edges != value)
                {
                    this._edges = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<ArtistViewModel> _artists;
        public List<ArtistViewModel> Artists
        {
            get { return _artists; }

            private set
            {
                if (_artists != value)
                {
                    this._artists = value; 
                    NotifyPropertyChanged();
                }
            }
        }

        public ArtistViewModel FocusedArtistViewModel
        {
            get;
            private set;
        }
        
        public List<ArtistHierarchy.Node> RootNodes
        {
            get
            {
                if (_hierarchy == null)
                    return null;

                return _hierarchy.RootNodes;
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Event Handlers
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        internal AtlasViewModel()
        {
            Title = "Sound Atlas";

            ArtistPanel tempArtistPanel = new ArtistPanel();
            tempArtistPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            _desiredArtistPanelSize = new Size(tempArtistPanel.MaxWidth, tempArtistPanel.DesiredSize.Height);
        }

        #region Hierarchy Functions
        public void Generate(IEnumerable<String> targetArtist)
        {
            _hierarchy = new ArtistHierarchy(SpotifyClientService.Client, targetArtist);

            List<ArtistViewModel> artistViewModels = GenerateHierarchy(_hierarchy);

            FocusedArtistViewModel = artistViewModels[0];
            FocusedArtistViewModel.IsSelected = true;

            Artists = artistViewModels;
        }

        public void GetPlacementCoordinates(int level, out Point location)
        {
            List<ArtistHierarchy.Node> levelSiblings = _hierarchy.GetNodesAtLevel(level);

            location = new Point();
            foreach (ArtistHierarchy.Node childNode in levelSiblings)
            {
                if (location.X < childNode.ArtistViewModel.Location.X)
                    location.X = childNode.ArtistViewModel.Location.X;

                if (location.Y < childNode.ArtistViewModel.Location.Y)
                    location.Y = childNode.ArtistViewModel.Location.Y;
            }

            if (location.Y > 0.0f)
                location.Y += _desiredArtistPanelSize.Height;
        }

        public void GenerateChildren(ArtistViewModel artistViewModel)
        {
            _hierarchy.GenerateSubTree(artistViewModel.HierarchyNode);
            artistViewModel.IsSelected = true;

            //Find the Y-value of the last chlidren in the list in the next level; append to the list there..
            Point maxLocation;
            GetPlacementCoordinates(artistViewModel.HierarchyNode.Level + 1, out maxLocation);

            Point startPoint = new Point(artistViewModel.Location.X, maxLocation.Y);
            List<ArtistViewModel> artistViewModels = GenerateChildren(startPoint, artistViewModel.HierarchyNode, false);

            List<ArtistViewModel> newList = new List<ArtistViewModel>(Artists);
            newList.AddRange(artistViewModels);

            //Regenerate whether the controls have been selected.  This could be improved ideally.
            //We would likely not want to create new view models repeatedly.
            foreach (ArtistViewModel viewModel in newList)
            {
                if (viewModel.HierarchyNode.Children.Any())
                    viewModel.IsSelected = true;
            }

            Artists = newList;
        }

        public void ClearChildren(ArtistViewModel artistViewModel)
        {
            artistViewModel.HierarchyNode.ClearSiblings();

            Point startPoint = new Point(artistViewModel.Location.X, 0);  //TODO:  Better way to handle these coordinates.
            List<ArtistViewModel> artistViewModels = GenerateHierarchy(_hierarchy);

            Artists = artistViewModels;
        }
        #endregion

        #region Private Hierarchy Generation Methods
        private List<ArtistViewModel> GenerateHierarchy(ArtistHierarchy hierarchy)
        {
            List<ArtistViewModel> artistViewModels = new List<ArtistViewModel>();

            Point startPoint = new Point();
            int heightPadding = 0;
            int offset = 0;

            foreach (ArtistHierarchy.Node rootNode in hierarchy.RootNodes)
            {
                Point location = new Point(startPoint.X, startPoint.Y + ((_desiredArtistPanelSize.Height + heightPadding) * offset));

                rootNode.ArtistViewModel.SetLocation(location);
                artistViewModels.Add(rootNode.ArtistViewModel);

                Point maxLocation;
                GetPlacementCoordinates(rootNode.Level + 1, out maxLocation);

                List<ArtistViewModel> points = GenerateChildren(new Point(startPoint.X, maxLocation.Y), rootNode, true);
                artistViewModels = artistViewModels.Concat(points).ToList();

                ++offset;
            }

            return artistViewModels;
        }

        private List<ArtistViewModel> GenerateChildren(Point point, ArtistHierarchy.Node node, bool generateChildren)
        {
            List<ArtistViewModel> artistViewModels = new List<ArtistViewModel>();

            int heightPadding = 0;
            int widthPadding = 150;

            if (node.Children.Any())
            {
                Point columnStart = new Point(point.X + _desiredArtistPanelSize.Width + widthPadding, point.Y);

                int offset = 0;
                foreach (ArtistHierarchy.Node artistNode in node.Children)
                {
                    Point location = new Point(columnStart.X, columnStart.Y + ((_desiredArtistPanelSize.Height + heightPadding) * offset));
                    artistNode.ArtistViewModel.SetLocation(location);
                    artistViewModels.Add(artistNode.ArtistViewModel);

                    if (generateChildren)
                    {
                        artistViewModels.AddRange(GenerateChildren(columnStart, artistNode, generateChildren));
                    }

                    ++offset;
                }
            }

            return artistViewModels;
        }
        #endregion
    }
}
