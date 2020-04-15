using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.control_station
{
    /// <summary>
    /// 控制副界面输入框输入操作
    /// </summary>
    class SecondaryInterInputControl : GeneralControlStation
    {
        /// <summary>
        /// 输入框初始化的字符串
        /// </summary>
        private string initText;
        /// <summary>
        /// 副界面输入框源
        /// </summary>
        private System.Windows.Controls.TextBox inputBoxObj;
        private bool initEnterFlag;
        public SecondaryInterInputControl(System.Windows.Controls.TextBox inputBoxObj, string initText, bool initEnterFlag)
        {
            this.initText = initText;
            this.initEnterFlag = initEnterFlag;
            this.inputBoxObj = inputBoxObj;
            this.inputBoxObj.Text = initText;
            this.inputBoxObj.GotFocus += InputBoxObj_GotFocus;
            this.inputBoxObj.KeyDown += InputBoxObj_EnterKeyDown;
        }

        private void InputBoxObj_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.initEnterFlag = false;
            if (inputBoxObj.Text == this.initText)
            {
                inputBoxObj.Text = "";
            }
        }

        /// <summary>
        /// 按下Enter键, 传递输入的文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputBoxObj_EnterKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key is System.Windows.Input.Key.Enter)
            {
                if (String.IsNullOrWhiteSpace(inputBoxObj.Text))
                {
                    return;
                }
                // 处理输入的询问文本
                new SecondaryInterSearchControl(inputBoxObj.Text.Trim(), inputBoxObj);
            }
        }

    }
}
