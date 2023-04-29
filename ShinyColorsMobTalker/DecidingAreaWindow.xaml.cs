using System;
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
using System.Windows.Shapes;

namespace ShinyColorsMobTalker
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class DecidingAreaWindow : Window
    {
        bool isWriting = false;
        Point Init;
        private List<UIElement> RectangleList = new List<UIElement>();
        UIElement RectElement = new UIElement();

        public DecidingAreaWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("button click");
            this.Close();
        }

        private void MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            Canvas c = sender as Canvas;
            Init = e.GetPosition(c);
            c.CaptureMouse();
            isWriting = true;
        }

        private void MouseLeftUp(object sender , MouseButtonEventArgs e)
        {
            if(isWriting)
            {
                Canvas c = sender as Canvas;
                isWriting = false;
                c.ReleaseMouseCapture();
                this.Close();
            }
        }

        private void WriteRectangle(Point point)
        {
            CanvasArea.Children.Remove(RectElement);
           

            Rectangle rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Red);
            rect.StrokeThickness = 1;

            rect.Width = Math.Abs(Init.X - point.X);
            rect.Height = Math.Abs(Init.Y - point.Y);

            if (point.X > CanvasArea.ActualWidth)
            {
                Canvas.SetLeft(rect, Init.X);
                rect.Width = CanvasArea.ActualWidth - Init.X;
            }
            else if (point.X < 0)
            {
                Canvas.SetLeft(rect, 0);
                rect.Width = Init.X;
            }
            else if (Init.X < point.X)
            {
                Canvas.SetLeft(rect, Init.X);
            }
            else
            {
                Canvas.SetLeft(rect, point.X);
            }

            if (point.Y > CanvasArea.ActualHeight)
            {
                Canvas.SetTop(rect, Init.Y);
                rect.Height = CanvasArea.ActualHeight - Init.Y;
            }
            else if (point.Y < 0)
            {
                Canvas.SetTop(rect, 0);
                rect.Height = Init.Y;
            }
            else if (Init.Y < point.Y)
            {
                Canvas.SetTop(rect, Init.Y);
            }
            else
            {
                Canvas.SetTop(rect, point.Y);
            }

            CanvasArea.Children.Add(rect);
            RectElement = rect;
        }

        private void MouseMoving(object sender, MouseEventArgs e)
        {
            if(isWriting)
            {
                Point pos = e.GetPosition(CanvasArea);
                WriteRectangle(pos);
            }
        }
    }
}
