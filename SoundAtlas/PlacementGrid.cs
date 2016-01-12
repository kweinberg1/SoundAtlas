using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SoundAtlas
{
    public class PlacementGrid
    {
        #region Properties
        private int Distance;
        #endregion

        public PlacementGrid(int distance)
        {
            Distance = distance;
        }

        /// <summary>
        /// Generates grid points based on a polygonal structure.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="controlSize"></param>
        /// <param name="numPoints"></param>
        /// <returns></returns>
        /*public Point[] GenerateGridPoints(Point startPoint, Size controlSize, int numPoints)
        {
            Point[] points = new Point[numPoints];

            int pointCount = 0;
            int iteration = 1;
            int radiusExtent = Distance;
            while (pointCount < numPoints)
            {
                int halfWidth = (int)controlSize.Width / 2;
                int halfHeight = (int)controlSize.Height / 2;

                float twoPi = (float)(Math.PI * 2.0);
                int numSides = LinksPerNode * iteration;
                for (int sideIndex = 0; sideIndex < numSides && pointCount < numPoints; ++sideIndex)
                {
                    float degrees = sideIndex * (twoPi / numSides);
                    float x = (float)Math.Sin(degrees);
                    float y = (float)Math.Cos(degrees);

                    points[pointCount].X = startPoint.X + (x * (radiusExtent + halfWidth));
                    points[pointCount].Y = startPoint.Y + (y * (radiusExtent + halfHeight));
                    pointCount++;
                }

                iteration++;
                radiusExtent = iteration * (Distance + halfHeight);
            }

            return points;
        }*/

        /// <summary>
        /// Generates points based on a tree structure.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="controlSize"></param>
        /// <param name="numPoints"></param>
        /// <returns></returns>
        public ArtistViewModel[] GenerateTreePoints(Size controlSize, ArtistHierarchy hierarchy)
        {
            List<ArtistViewModel> artistViewModels = new List<ArtistViewModel>();

            Point startPoint = new Point();

            hierarchy.Root.ArtistViewModel.SetLocation(startPoint);
            artistViewModels.Add(hierarchy.Root.ArtistViewModel);

            List<ArtistViewModel> points = GenerateTreePoints(startPoint, controlSize, hierarchy.Root);
            artistViewModels = artistViewModels.Concat(points).ToList();  //Bleh...            

            //Offset this so all view models are in positive space.
            Point minPoint = new Point(double.MaxValue, double.MaxValue);
            Point maxPoint = new Point(double.MinValue, double.MinValue);

            foreach (ArtistViewModel viewModel in artistViewModels)
            {
                if (viewModel.X < minPoint.X)
                    minPoint.X = viewModel.X;

                if (viewModel.Y < minPoint.Y)
                    minPoint.Y = viewModel.Y;

                if (viewModel.X > maxPoint.X)
                    maxPoint.X = viewModel.X;

                if (viewModel.Y > maxPoint.Y)
                    maxPoint.Y = viewModel.Y;
            }

            double offsetWidth = (maxPoint.X - minPoint.X);
            double offsetHeight = (maxPoint.Y - minPoint.Y);

            foreach (ArtistViewModel viewModel in artistViewModels)
            {
                viewModel.X += offsetWidth;
                viewModel.Y += offsetHeight;
            }

            return artistViewModels.ToArray();
        }

        /// <summary>
        /// Generates points to place a tree visually.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="controlSize"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<ArtistViewModel> GenerateTreePoints(Point point, Size controlSize, ArtistHierarchy.Node node)
        {
            List<ArtistViewModel> artistViewModels = new List<ArtistViewModel>();

            int padding = 0; //TODO: Data-drive.

            if (node.Children.Any())
            {
                //Determine the length of the entire row.
                double rowWidth = (controlSize.Width * node.Children.Count()) + (padding * node.Children.Count());
                Point rowStart = new Point(point.X - (rowWidth / 2.0), point.Y + Distance);

                int offset = 0;
                foreach (ArtistHierarchy.Node artistNode in node.Children)
                {
                    Point location = new Point(rowStart.X + ((controlSize.Width + padding) * offset), rowStart.Y);
                    artistNode.ArtistViewModel.SetLocation(location);
                    artistViewModels.Add(artistNode.ArtistViewModel);

                    offset++;
                }

                foreach (ArtistHierarchy.Node artistNode in node.Children)
                {
                    Point currentPoint = new Point(point.X, point.Y + Distance);
                    List<ArtistViewModel> points = GenerateTreePoints(currentPoint, controlSize, artistNode);
                    artistViewModels = artistViewModels.Concat(points).ToList();  //Bleh...       

                    break; //This is a dirty hack.
                }
            }

            return artistViewModels;
        }

        public List<ArtistViewModel> GenerateColumnPoints(Size controlSize, ArtistHierarchy hierarchy)
        {
            List<ArtistViewModel> artistViewModels = new List<ArtistViewModel>();

            Point startPoint = new Point();

            hierarchy.Root.ArtistViewModel.SetLocation(startPoint);
            artistViewModels.Add(hierarchy.Root.ArtistViewModel);

            List<ArtistViewModel> points = GenerateColumnPoints(startPoint, controlSize, hierarchy.Root, true);
            artistViewModels = artistViewModels.Concat(points).ToList();

            return artistViewModels;
        } 

        public List<ArtistViewModel> GenerateColumnPoints(Point point, Size controlSize, ArtistHierarchy.Node node, bool generateChildren)
        {
            List<ArtistViewModel> artistViewModels = new List<ArtistViewModel>();

            int heightPadding = 0;

            if (node.Children.Any())
            {
                Point columnStart = new Point(point.X + Distance + controlSize.Width, point.Y);

                int offset = 0;
                foreach (ArtistHierarchy.Node artistNode in node.Children)
                {
                    Point location = new Point(columnStart.X, columnStart.Y + ((controlSize.Height + heightPadding) * offset));
                    artistNode.ArtistViewModel.SetLocation(location);
                    artistViewModels.Add(artistNode.ArtistViewModel);

                    if (generateChildren)
                    {
                        artistViewModels.AddRange(GenerateColumnPoints(columnStart, controlSize, artistNode, generateChildren));
                    }

                    ++offset;
                }
            }

            return artistViewModels;
        }
    }
}
