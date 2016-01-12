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

namespace SoundAtlas
{
    /// <summary>
    /// Interaction logic for ScrollableCanvas.xaml
    /// </summary>
    public partial class ScrollableCanvas : Canvas
    {
        public ScrollableCanvas()
        {
            InitializeComponent();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double bottomMost = 0d;
            double rightMost = 0d;

            foreach (object obj in this.Children)
            {
                FrameworkElement child = obj as FrameworkElement;

                if (child != null)
                {
                    child.Measure(constraint);

                    bottomMost = Math.Max(bottomMost, GetTop(child) + child.DesiredSize.Height);
                    rightMost = Math.Max(rightMost, GetLeft(child) + child.DesiredSize.Width);
                }
            }

            if (double.IsNaN(rightMost))
                rightMost = 0d;
            if (double.IsNaN(bottomMost))
                bottomMost = 0d;

            return new Size(rightMost, bottomMost);
        }
    }
}
