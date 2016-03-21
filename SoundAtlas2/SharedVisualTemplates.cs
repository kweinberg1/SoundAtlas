namespace SoundAtlas2
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Shapes;

    public partial class SharedVisualTemplates : ResourceDictionary
    {
        private bool _resizing = false;

        private void OnResizeInit(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            _resizing = true;
            rect.CaptureMouse();
        }

        private void OnResizeEnd(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            _resizing = false; ;
            rect.ReleaseMouseCapture();
        }

        private void OnResizeMove(object sender, MouseEventArgs e)
        {
            if (!_resizing)
                return;

            Rectangle senderRect = sender as Rectangle;
            Window mainWindow = senderRect.Tag as Window;

            if (mainWindow != null)
                return;

            double width = e.GetPosition(mainWindow).X;
            double height = e.GetPosition(mainWindow).Y;
            senderRect.CaptureMouse();
            if (senderRect.HorizontalAlignment == HorizontalAlignment.Right)
            {
                width += 5;
                if (width > 0)
                    mainWindow.Width = width;
            }
            if (senderRect.HorizontalAlignment == HorizontalAlignment.Left)
            {
                width -= 5;
                mainWindow.Left += width;
                width = mainWindow.Width - width;
                if (width > 0)
                {
                    mainWindow.Width = width;
                }
            }
            if (senderRect.VerticalAlignment == VerticalAlignment.Bottom)
            {
                height += 5;
                if (height > 0)
                    mainWindow.Height = height;
            }
            if (senderRect.VerticalAlignment == VerticalAlignment.Top)
            {
                height -= 5;
                mainWindow.Top += height;
                height = mainWindow.Height - height;
                if (height > 0)
                {
                    mainWindow.Height = height;
                }
            }
        }

        private void OnMaximizeDragGripLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle senderRect = sender as Rectangle;
            Window mainWindow = senderRect.Tag as Window;

            if (e.ClickCount > 1)
            {
                if (mainWindow.WindowState == System.Windows.WindowState.Normal)
                {
                    mainWindow.WindowState = System.Windows.WindowState.Maximized;
                }
                else
                {
                    mainWindow.WindowState = System.Windows.WindowState.Normal;
                }
            }

            mainWindow.DragMove(); 
        }
    }
}
    