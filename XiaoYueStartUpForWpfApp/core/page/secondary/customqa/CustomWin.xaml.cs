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

namespace XiaoYueStartUpForWpfApp.core.page.secondary.customqa
{
    /// <summary>
    /// CustomWin.xaml 的交互逻辑
    /// 显示自定义问答编辑界面
    /// </summary>
    /// TODO 允许用户删除之前添加的每个自定义问答
    public partial class CustomWin : Window
    {
        private int _doubleSpace = 0;
        private Window customReplyDialog;
        private TextBox replyTextBox;
        private string replyText;
        public CustomWin()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.Height;
            this.textBox.PreviewKeyDown += TextBox_PreviewKeyDown; ;
            this.textBtn.Click += TextBtn_Click;
            this.soundBtn.Click += SoundBtn_Click;
            this.imgBtn.Click += ImgBtn_Click;
        }

        /// <summary>
        /// 选择了以文本方式自定义回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBox.Text.Trim()))
            {
                return;
            }
            string customQ = textBox.Text.Trim();
            // 去除以下标点符号, 但不去除空格
            String[] splitStrs = customQ.Split(new char[] {
                ',', '.', '/', ';', '\'', '?', '"', ':', '!', '-', '+',
                '=', '*', '/', '@', '#', '$', '%', '^', '&', '(', ')',
                '~', '|', '\\', '`', '。', '，', '！', '？', '“', '”',
                '：', '；', '‘', '’', '【', '】', '{', '}', '、', '￥',
                '_', '<', '>', '《', '》', '·', '…', '[', ']', '（', '）',
                '—' }
            );
            IEnumerable<string> i = splitStrs.Take(splitStrs.Length);
            IEnumerator<string> enumerator = i.GetEnumerator();
            customQ = "";
            while (enumerator.MoveNext())
            {
                customQ += enumerator.Current;
            }
            enumerator.Dispose();
            Console.WriteLine(customQ);

            //显示自定义回复的编辑框
            customReplyDialog = new Window();
            customReplyDialog.Width = 300;
            customReplyDialog.SizeToContent = SizeToContent.Height;
            customReplyDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            customReplyDialog.WindowState = WindowState.Normal;
            customReplyDialog.WindowStyle = WindowStyle.ToolWindow;
            customReplyDialog.ResizeMode = ResizeMode.NoResize;
            customReplyDialog.ShowInTaskbar = false;

            StackPanel stackPanel = new StackPanel();
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Center;

            replyTextBox = new TextBox();
            replyTextBox.HorizontalAlignment = HorizontalAlignment.Center;
            replyTextBox.VerticalAlignment = VerticalAlignment.Center;
            replyTextBox.MaxLength = 100;
            replyTextBox.FontSize = 14;
            replyTextBox.Width = 220;
            replyTextBox.Height = 140;
            replyTextBox.TextWrapping = TextWrapping.Wrap;
            replyTextBox.TextAlignment = TextAlignment.Justify;

            Button okBtn = new Button();
            okBtn.Margin = new Thickness(0, 10, 0, 10);
            okBtn.Width = 60;
            okBtn.Height = 40;
            okBtn.Content = "确定";
            okBtn.FontSize = 16;
            okBtn.Background = Brushes.AliceBlue;
            okBtn.Foreground = Brushes.Black;
            okBtn.HorizontalAlignment = HorizontalAlignment.Center;
            okBtn.VerticalAlignment = VerticalAlignment.Center;
            okBtn.VerticalContentAlignment = VerticalAlignment.Center;
            okBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
            okBtn.Click += OkBtn_Click;

            stackPanel.Children.Add(replyTextBox);
            stackPanel.Children.Add(okBtn);

            customReplyDialog.Content = stackPanel;
            customReplyDialog.Title = "编辑XiaoYue的回复";
            if ((bool)customReplyDialog.ShowDialog())
            {
                // 控制器操作 添加至本地数据文件
                Console.WriteLine(customQ);
                Console.WriteLine(this.replyText);
                new control_station.CustomQAControl(customQ, this.replyText);
            }
        }

        /// <summary>
        /// 当编辑了自定义回复后确定按钮按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(replyTextBox.Text))
            {
                return;
            }
            this.replyText = replyTextBox.Text;
            customReplyDialog.DialogResult = true;
            customReplyDialog.Close();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 静止粘贴
            if (e.Key == Key.V && ((Keyboard.Modifiers & ModifierKeys.Control) > 0))
            {
                e.Handled = true;
            }

            if (e.Key == Key.Space)
            {
                // 禁止连续两个及以上的空格键输入(之后还要去除任何标点符号, 但不去除中间空格)
                if (_doubleSpace == 0)
                {
                    _doubleSpace += 1;
                }
                else if (_doubleSpace == 1)
                {
                    e.Handled = true;
                }
            }
            else
            {
                _doubleSpace = 0;
            }
        }

        /// <summary>
        /// 选择了以音频方式自定义回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgBtn_Click(object sender, RoutedEventArgs e)
        {
            //todo
        }

        /// <summary>
        /// 选择了以图片方式自定义回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SoundBtn_Click(object sender, RoutedEventArgs e)
        {
            //todo
        }
    }
}
