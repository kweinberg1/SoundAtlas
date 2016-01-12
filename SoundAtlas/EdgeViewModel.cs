using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoundAtlas
{
    /// <summary>
    /// 
    /// </summary>
    public partial class EdgeViewModel
    {
        private ArtistPanel _Source;
        private ArtistPanel _Target;
        private Atlas _Parent;

        public EdgeViewModel(ArtistPanel source, ArtistPanel target, Atlas parent)
        {
            _Source = source;
            _Target = target;
            _Parent = parent;
        }

        public Point Location
        {
            get
            {
                Point linkPoint = new Point(0.0, 0.0);
                GeneralTransform trans = _Source.TransformToAncestor(_Parent);
                Rect slot = LayoutInformation.GetLayoutSlot(_Source);
                Point sourceLocation = trans.Transform(new Point(slot.Right, slot.Bottom / 2.0));                
                return sourceLocation;
            }
        }

        public Point TargetLocation
        {
            get
            {
                Point linkPoint = new Point(0.0, 0.0);
                GeneralTransform trans = _Target.TransformToAncestor(_Parent);
                Rect slot = LayoutInformation.GetLayoutSlot(_Target);
                Point targetLocation = trans.Transform(new Point(slot.Left, slot.Bottom / 2.0));
                targetLocation.X -= Location.X;
                targetLocation.Y -= Location.Y;
                return targetLocation;
            }
        }


        /*protected override void OnRender(DrawingContext drawingContext)
        {
            //Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);

            Rect adornedElementRect = VisualTreeHelper.GetContentBounds(AdornedElement);


            Point sourceLocation = _Source.TranslatePoint(new Point(0, 0), AdornedElement);
            sourceLocation.X += _Source.RenderSize.Width;
            sourceLocation.Y += _Source.RenderSize.Height / 2.0;

            Point targetLocation = _Target.TranslatePoint(new Point(0, 0), AdornedElement);
            targetLocation.Y += _Target.RenderSize.Height / 2.0;

            drawingContext.DrawLine(renderPen, new Point(adornedElementRect.Left + sourceLocation.X, adornedElementRect.Top + sourceLocation.Y), new Point(adornedElementRect.Left + targetLocation.X, adornedElementRect.Top + targetLocation.Y));
        }*/
    }
}
