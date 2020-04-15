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
    /// SpeechTextDisplay.xaml 的交互逻辑
    /// 语音输入完成后显示的应答界面
    /// </summary>
    public partial class SpeechTextDisplay : Window
    {
        public SpeechTextDisplay(string speechText, string answerText)
        {
            InitializeComponent();
            this.your_msg_display.Text = speechText;
            this.xy_msg_display.Text = answerText;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
