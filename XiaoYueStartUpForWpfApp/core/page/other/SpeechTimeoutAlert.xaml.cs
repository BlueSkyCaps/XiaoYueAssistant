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
using System.Windows.Shapes;

namespace XiaoYueStartUpForWpfApp.core.page.other
{
    /// <summary>
    /// SpeechTimeoutAlert.xaml 的交互逻辑
    /// </summary>
    public partial class SpeechTimeoutAlert : Window
    {
        public SpeechTimeoutAlert()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SizeToContent = SizeToContent.WidthAndHeight;
            Topmost = true;
            WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            System.Windows.Controls.Label textLabel = new System.Windows.Controls.Label
            {
                Width = 250,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Background = System.Windows.Media.Brushes.BlueViolet,
                Foreground = System.Windows.Media.Brushes.White,
                Content = "OMG!说话太久了..",
                FontSize = 25,
                FontWeight = System.Windows.FontWeights.ExtraBlack,
                FontStyle = System.Windows.FontStyles.Normal,
            };
            System.Windows.Controls.StackPanel stackPanel = new System.Windows.Controls.StackPanel();
            stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Center;
            stackPanel.Children.Add(textLabel);
            Content = stackPanel;
        }
    }
}
