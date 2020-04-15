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

namespace XiaoYueStartUpForWpfApp.core.page.secondary.recognition
{
    /// <summary>
    /// RecognitionResultWin.xaml 的交互逻辑
    /// </summary>
    public partial class RecognitionResultWin : Window
    {
        public RecognitionResultWin(string conStr, string imgPath)
        {
            InitializeComponent();
            BitmapImage bitmapImage = new BitmapImage(new Uri(imgPath, UriKind.Absolute));
            ImageBrush imgBrush = new ImageBrush(bitmapImage);
            imgBrush.Stretch = Stretch.Uniform;
            this.presentImg.Background = imgBrush;
            this.textData.Text = conStr;
            this.Show();
        }
    }
}
