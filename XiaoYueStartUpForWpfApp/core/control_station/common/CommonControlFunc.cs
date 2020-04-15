using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace XiaoYueStartUpForWpfApp.core.control_station.common
{
    /// <summary>
    /// control_station下所有类会用到的公有方法
    /// 全部方法应为静态方法
    /// 供每个类直接调用实现所需的功能
    /// </summary>
    class CommonControlFunc
    {
        public CommonControlFunc() {}

        /// <summary>
        /// 创建全屏的透明遮罩层, 当用户进行语音命令时
        /// </summary>
        /// <returns>mask: Window对象</returns>
        /// 已替换Form窗体。因为MediaElement无法适用
        public static Window MakeTansparentScreen()
        {
            Window mask = new Window();
            StackPanel stackPanel = new StackPanel();
            stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            MediaElement mediaElement = new MediaElement();
            mediaElement.MediaEnded += MediaElement_MediaEnded;
            mediaElement.Clock = null;
            mediaElement.Stretch = System.Windows.Media.Stretch.Uniform;
            mediaElement.VerticalAlignment = VerticalAlignment.Center;
            mediaElement.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            mediaElement.Source = new Uri("resources/animation/media/listen-to-your-orders.wmv", UriKind.Relative);

            stackPanel.Children.Add(mediaElement);
            mask.Content = stackPanel;

            mask.AllowsTransparency = true;
            mask.BorderThickness = new System.Windows.Thickness(.0);
            mask.WindowStyle = WindowStyle.None;
            mask.ShowInTaskbar = false;
            mask.WindowState = WindowState.Maximized;
            mask.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mask.Opacity = .9;
            mask.Background = System.Windows.Media.Brushes.Black;

            mask.Show();
            // Cursor.Hide(); /*注: 此处不应该隐藏光标, 因为当前窗体会返回给调用方。即使调用方Cursor.Show()也无法显示光标,*/
            return mask;
        }

        private static void MediaElement_MediaEnded(object sender, RoutedEventArgs e) {}
    }
}
