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

namespace XiaoYueStartUpForWpfApp.core.page.secondary.task
{
    /// <summary>
    /// AlarmReminderWindow.xaml 的交互逻辑
    /// 闹钟以及提醒的设置界面(关机和锁屏设置界面也基于此)
    /// 此类应是个对话框窗口, 因为调用方需要ShowDialog()来获取结果
    /// </summary>
    public partial class AlarmReminderWindow : Window
    {
        /// <summary>
        /// 供TaskTimingWindow访问的字段
        /// </summary>
        public static int sliderHour, sliderMinute;
        public AlarmReminderWindow()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.slider_hour.ValueChanged += Slider_hour_ValueChanged;
            this.slider_minute.ValueChanged += Slider_minute_ValueChanged;
            this.ok_return_btn.Click += Ok_return_btn_Click;
        }

        private void Ok_return_btn_Click(object sender, RoutedEventArgs e)
        {
            sliderHour = int.Parse(this.slider_hour.Value.ToString());
            sliderMinute = int.Parse(this.slider_minute.Value.ToString());
            // 返回True以供调用方判断
            this.DialogResult = true;
            this.Close();
        }

        private void Slider_minute_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.label_time.Content = this.slider_hour.Value + ":" + this.slider_minute.Value;
        }

        private void Slider_hour_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.label_time.Content = this.slider_hour.Value + ":" + this.slider_minute.Value;
        }
    }
}
