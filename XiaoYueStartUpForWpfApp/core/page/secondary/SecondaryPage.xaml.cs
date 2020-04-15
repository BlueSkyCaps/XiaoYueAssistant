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
    /// SecondaryPage.xaml 的交互逻辑
    /// 一些准备化工作, 以及响应SecondaryPage的子控件
    /// 将逻辑转交给控制器类
    /// 此类跟随SecondaryInter类的生成而加载
    /// </summary>
    public partial class SecondaryPage : Page
    {
        private static bool ShowDlgOnce = false;
        private static secondary.task.TaskTimingWindow taskTimingWindow;
        public SecondaryPage()
        {
            InitializeComponent();
            // 初始化输入框的一些准备工作, 包括跟踪事件、文本值等
            InitUserInputPreparation();
        }

        private void InitUserInputPreparation()
        {
            string str = "    输入想问的问题，或聊你所想！";
            // 生成userInputMessage控件的控制器
            new control_station.SecondaryInterInputControl(this.userInputMessage, str, true);
        }

        /// <summary>
        /// 点击了item1_timing_task(Header=="魔法训练") 打开CustomWin界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item1_training_magic_Click(object sender, RoutedEventArgs e)
        {
            new customqa.CustomWin().Show();
        }

        /// <summary>
        /// 点击了item2_timing_task(Header=="定时任务") 打开TaskTimingWindow界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item2_timing_task_Click(object sender, RoutedEventArgs e)
        {
            if (!ShowDlgOnce)
            {
                taskTimingWindow = new secondary.task.TaskTimingWindow();
                taskTimingWindow.ShowDialog();
                ShowDlgOnce = true;
            }
            else {
                taskTimingWindow.ShowDialog();
                ShowDlgOnce = true;
            }
        }

        /// <summary>
        /// 点击了Item3_image_recognition(Header=="图像识别") 打开图像识别选项界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item3_image_recognition_Click(object sender, RoutedEventArgs e)
        {
            new secondary.recognition.RecognitionMenu().Show();
        }
    }
}
