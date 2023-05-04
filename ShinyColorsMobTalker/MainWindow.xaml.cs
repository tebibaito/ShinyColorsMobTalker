using ShinyColorsMobTalker.Models;
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

namespace ShinyColorsMobTalker
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private CommonModel commonModel;

        public MainWindow()
        {
            InitializeComponent();
            commonModel = CommonModel.GetInstance();
            commonModel.StartMainProc();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var win = new DecidingAreaWindow();
            win.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CommonModel.GetInstance().ScreenShot();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CommonModel.GetInstance().ToggleProcess();
        }
    }
}
