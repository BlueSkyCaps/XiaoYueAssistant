using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TextBox = System.Windows.Controls.TextBox;
using Button = System.Windows.Controls.Button;

namespace XiaoYueStartUpForWpfApp.core.page.secondary.task
{
    /// <summary>
    /// TaskTimingWindow.xaml 的交互逻辑
    /// 用于进行定时任务功能的界面
    /// </summary>
    /* 说明: 用户最多只能设置三个定时任务。退出主程序后, 将不保存数据且所有定时任务的线程被释放 */

    public partial class TaskTimingWindow : Window
    {
        public TaskTimingWindow()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.Height;
        }

        /* 以下是此类中一些控件的响应事件逻辑 */
        
        /// <summary>
        /// 当task_scbtn_one等按钮点击时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Task_scbtn_one_Click(object sender, RoutedEventArgs e)
        {
            CheckLastValue(task_combobox_one, task_textbox_one, task_pgh_one, task_scbtn_one);
        }

        private void Task_scbtn_two_Click(object sender, RoutedEventArgs e)
        {
            CheckLastValue(task_combobox_two, task_textbox_two, task_pgh_two, task_scbtn_two);
        }

        private void Task_scbtn_three_Click(object sender, RoutedEventArgs e)
        {
            CheckLastValue(task_combobox_three, task_textbox_three, task_pgh_three, task_scbtn_three);
        }
        /// <summary>
        /// 当open_task_btn_one等按钮点击时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_task_btn_one_Click(object sender, RoutedEventArgs e)
        {
            open_task_btn_one.Visibility = Visibility.Collapsed;
        }


        private void Open_task_btn_two_Click(object sender, RoutedEventArgs e)
        {
            open_task_btn_two.Visibility = Visibility.Collapsed;
        }


        private void Open_task_btn_three_Click(object sender, RoutedEventArgs e)
        {
            open_task_btn_three.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 当选择的定时任务类型的值改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Task_combobox_one_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UnifiedLogicCombobox(task_combobox_one);
        }

        private void Task_combobox_two_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UnifiedLogicCombobox(task_combobox_two);
        }

        private void Task_combobox_three_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UnifiedLogicCombobox(task_combobox_three);
        }

        /// <summary>
        /// Combobox对象下的统一逻辑-复用函数 
        /// <paramref name="whatComboBox">ComboBox的源对象</paramref>
        /// </summary>
        private void UnifiedLogicCombobox(System.Windows.Controls.ComboBox whatComboBox)
        {
            switch (whatComboBox.SelectedIndex)
            {
                case 0:
                    bool? arwDialog = new page.secondary.task.AlarmReminderWindow().ShowDialog();
                    if (arwDialog == false)
                    {
                        // 如果没有选择任何内容, 选定隐藏的项以提醒用户 
                        whatComboBox.Text = " "; /* 注: 这行代码是必须的, 因为如果连续多次取消了对话框, 下一行代码不起作用 */
                        whatComboBox.Text = "无任何内容";
                    }
                    else
                    {
                        string stime = AlarmReminderWindow.sliderHour + ":" + AlarmReminderWindow.sliderMinute;
                        if (whatComboBox.Items.Contains(stime + "|闹钟"))
                        {
                            whatComboBox.Items.Remove(stime + "|闹钟");
                        }
                        whatComboBox.Items.Add(stime+"|闹钟");
                        whatComboBox.Text = " ";
                        whatComboBox.Text = stime + "|闹钟";
                    }
                    break;

                case 1:
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
                    {
                        InitialDirectory = "C:\\",
                        Title = "选择一个需要定时打开的应用",
                        Multiselect = false,
                        DefaultExt = ".exe",
                        Filter = "可执行文件|*.exe; *.msi",
                        DereferenceLinks = true // 返回选定的快捷方式引用的文件位置
                    };
                    bool? result = dlg.ShowDialog();
                    if (result == true)
                    {
                        string filePath = dlg.FileName;
                        string saveFileName = dlg.SafeFileName;
                        // 此操作还需要用户设置时间
                        bool? openDialogSetTime = new page.secondary.task.AlarmReminderWindow().ShowDialog();
                        if (openDialogSetTime == false)
                        {
                            whatComboBox.Text = " ";
                            whatComboBox.Text = "无任何内容";
                            break;
                        }
                        string stime = AlarmReminderWindow.sliderHour + ":" + AlarmReminderWindow.sliderMinute;
                        if (whatComboBox.Items.Contains(stime + "|打开|" + filePath))
                        {
                            whatComboBox.Items.Remove(stime + "|打开|" + filePath);
                        }
                        whatComboBox.Items.Add(stime+"|打开|"+ filePath);
                        whatComboBox.Text = " ";
                        whatComboBox.Text = stime + "|打开|" + filePath;
                    }
                    else if (result == false)
                    {
                        whatComboBox.Text = " ";
                        whatComboBox.Text = "无任何内容";
                    }
                    break;

                case 2:
                    bool? shutDialog = new page.secondary.task.AlarmReminderWindow().ShowDialog();
                    if (shutDialog == false)
                    {
                        whatComboBox.Text = " ";
                        whatComboBox.Text = "无任何内容";
                    }
                    else
                    {
                        string stime = AlarmReminderWindow.sliderHour + ":" + AlarmReminderWindow.sliderMinute;
                        if (whatComboBox.Items.Contains(stime + "|关机"))
                        {
                            whatComboBox.Items.Remove(stime + "|关机");
                        }
                        whatComboBox.Items.Add(stime+"|关机");
                        whatComboBox.Text = " ";
                        whatComboBox.Text = stime + "|关机";
                    }
                    break;

                case 3:
                    bool? lockDialog = new page.secondary.task.AlarmReminderWindow().ShowDialog();
                    if (lockDialog == false)
                    {
                        whatComboBox.Text = " ";
                        whatComboBox.Text = "无任何内容";
                    }
                    else
                    {
                        string stime = AlarmReminderWindow.sliderHour + ":" + AlarmReminderWindow.sliderMinute;
                        if (whatComboBox.Items.Contains(stime + "|锁屏"))
                        {
                            whatComboBox.Items.Remove(stime + "|锁屏");
                        }
                        whatComboBox.Items.Add(stime + "|锁屏");
                        whatComboBox.Text = " ";
                        whatComboBox.Text = stime + "|锁屏";
                    }
                    break;
                default: break;
            }
        }

        /// <summary>
        /// 检查用户是否设置了内容以提交
        /// </summary>
        /// <param name="whatComboBox">ComboBox的源对象</param>
        private void CheckLastValue(System.Windows.Controls.ComboBox whatComboBox, TextBox whatTextBox, Paragraph whatParagraph, System.Windows.Controls.Button scbutton)
        {
            if (whatComboBox.SelectedIndex == -1 || whatComboBox.Text == "无任何内容")
            {
                System.Windows.Forms.MessageBox.Show("请设置类型以及时间..", "提示", MessageBoxButtons.OK);
                return;
            }

            // 判断触发按钮Content是"开始"还是"撤销"
            if ((string)scbutton.Content == "开始")
            {
                // 传递所需数据
                string time = whatComboBox.Text.Split(new char[] {'|'})[0]; // 任务触发的时间
                bool makeRst = new control_station.TaskTimingWindowControl().MakeTaskThread(scbutton, time, whatComboBox.Text, whatTextBox.Text, new TextRange(whatParagraph.ContentStart, whatParagraph.ContentEnd).Text);
                if (makeRst)
                {
                    scbutton.Content = "撤销"; // 此时用户可再次点击以撤销任务
                }
            }
            else
            {
                bool cancelRst = new control_station.TaskTimingWindowControl().CancelTheTaskThread(scbutton);
                if (cancelRst)
                {
                    scbutton.Content = "开始"; // 此时用户可从新设置任务以开始
                }
            }
        }


        /// <summary>
        /// 隐藏窗口而不是关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hide_lb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
             this.DragMove();
        }

        /// <summary>
        /// 用于重绘界面。防止退出主程序后再次打开时, 已设置的任务数据丢失
        /// 此函数由TaskTimingWindowControl类调用
        /// </summary>
        // TODO (目前不需要)
        public static void RepaintSelection() {
            // TODO
        }
    }
}
