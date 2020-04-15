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

namespace XiaoYueStartUpForWpfApp.core.page.secondary
{
    /// <summary>
    /// SecondaryInter.xaml 的交互逻辑
    /// </summary>
    public partial class SecondaryInter : NavigationWindow
    {
        public SecondaryInter()
        {
            InitializeComponent();
            this.Source = new Uri("SecondaryPage.xaml", UriKind.Relative); //!!!注: 此行会引发异常, 但程序无任何问题, 考虑wpf界面
            // 匹配子页面内容大小
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            this.Activate();
        }

        /// <summary>
        /// 响应leftBtnForDragContextLabel拖动副界面窗口
        /// </summary>
        /// <param name="leftBtnForDragContextLabel">拖动区域用于拖动整个窗口</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs leftBtnForDragContextLabel)
        {
            if (leftBtnForDragContextLabel.Source is Label contextLabelByTrans)
                try
                {
                    this.DragMove();
                }
                catch (InvalidOperationException) {}
        }

        /// <summary>
        /// DragContextLabel被右键单击时, 隐藏副界面, 生成主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source is Label contextLabelByTrans)
            {
                new core.run.MainSteeringWheel();
                this.Hide();
            }
        }
    }
}
