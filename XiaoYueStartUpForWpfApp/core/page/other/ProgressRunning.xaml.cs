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
    /// ProgressRunning.xaml 的交互逻辑
    /// 显示进度条。当后台执行响应逻辑等时
    /// </summary>
    public partial class ProgressRunning : Window
    {
        public ProgressRunning()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }
    }
}
