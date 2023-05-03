using ShinyColorsMobTalker.Models;
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
using Windows.Media.Ocr;

namespace ShinyColorsMobTalker
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class DecidingAreaWindow : Window
    {
        private bool isWriting = false;
        private Point initPoint;
        private Point endPoint;
        private List<UIElement> RectangleList = new List<UIElement>();
        private UIElement RectElement = new UIElement();

        private CommonModel commonModel;


        public DecidingAreaWindow()
        {
            InitializeComponent();
            commonModel = CommonModel.GetInstance();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("button click");
            this.Close();
        }

        private void MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            Canvas c = sender as Canvas;
            initPoint = e.GetPosition(c);
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

                // マウスクリックUP時の座標を画面内に収める
                if(endPoint.X > CanvasArea.ActualWidth)
                {
                    endPoint.X = CanvasArea.ActualWidth;
                }
                else if(endPoint.X < 0)
                {
                    endPoint.X = 0;
                }
                if(endPoint.Y > CanvasArea.ActualHeight)
                {
                    endPoint.Y = CanvasArea.ActualHeight;
                }
                else if(endPoint.Y < 0)
                {
                    endPoint.Y = 0;
                }

                //マウスのスクリーン座標を取得
                Point initScreenPoint = PointToScreen(initPoint);
                Point endScreenPoint = PointToScreen(endPoint);

                double width = Math.Abs(initScreenPoint.X - endScreenPoint.X);
                double height = Math.Abs(initScreenPoint.Y - endScreenPoint.Y);
                double leftTopX = initScreenPoint.X <= endScreenPoint.X ? initScreenPoint.X : endScreenPoint.X;
                double leftTopY = initScreenPoint.Y <= endScreenPoint.Y ? initScreenPoint.Y : endScreenPoint.Y;

                commonModel.SetLeftTopX(leftTopX);
                commonModel.SetLeftTopY(leftTopY);
                commonModel.SetWidth(width);
                commonModel.SetHeight(height);

                commonModel.InitScreenShot();
                
                Debug.Print($"leftTopX:{leftTopX}, leftTopY:{leftTopY}, width:{width}, height:{height}");

                this.Close();
            }
        }

        private void WriteRectangle(Point point)
        {
            CanvasArea.Children.Remove(RectElement);
           

            Rectangle rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Red);
            rect.StrokeThickness = 1;

            rect.Width = Math.Abs(initPoint.X - point.X);
            rect.Height = Math.Abs(initPoint.Y - point.Y);

            if (point.X > CanvasArea.ActualWidth)
            {
                Canvas.SetLeft(rect, initPoint.X);
                rect.Width = CanvasArea.ActualWidth - initPoint.X;                
            }
            else if (point.X < 0)
            {
                Canvas.SetLeft(rect, 0);
                rect.Width = initPoint.X;
            }
            else if (initPoint.X < point.X)
            {
                Canvas.SetLeft(rect, initPoint.X);
            }
            else
            {
                Canvas.SetLeft(rect, point.X);
            }

            if (point.Y > CanvasArea.ActualHeight)
            {
                Canvas.SetTop(rect, initPoint.Y);
                rect.Height = CanvasArea.ActualHeight - initPoint.Y;
            }
            else if (point.Y < 0)
            {
                Canvas.SetTop(rect, 0);
                rect.Height = initPoint.Y;
            }
            else if (initPoint.Y < point.Y)
            {
                Canvas.SetTop(rect, initPoint.Y);
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
                endPoint = pos;
                WriteRectangle(pos);
            }
        }
    }
}
