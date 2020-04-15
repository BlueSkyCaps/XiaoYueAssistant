
using System;
using System.Windows;
using System.Windows.Input;

namespace XiaoYueStartUpForWpfApp.core.page.secondary.recognition
{
    /// <summary>
    /// RecognitionMenu.xaml 的交互逻辑
    /// 显示图像识别的一些选项
    /// </summary>
    public partial class RecognitionMenu : Window
    {
        /// <summary>
        /// 鼠标左键是否按下了选项以继续
        /// </summary>
        private bool _pre_mouseLDown = false;
        public RecognitionMenu()
        {
            InitializeComponent();
            this.Focus();
            this.Deactivated += RecognitionMenu_Deactivated; 
            this.image_recog.MouseEnter += Image_recog_MouseEnter;
            this.textimg_recog.MouseEnter += Face_recog_MouseEnter;
            this.image_recog.MouseLeftButtonDown += new MouseButtonEventHandler(ImageRecogMouseLeftDown);
            this.textimg_recog.MouseLeftButtonDown += new MouseButtonEventHandler(Textimg_recog_MouseDown);
        }

        private void ImageRecogMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            _pre_mouseLDown = true;
            Microsoft.Win32.OpenFileDialog openImgDialog = new Microsoft.Win32.OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "图像识别：选择一张图片",
                Multiselect = false,
                Filter = "图片图像|*.png; *.jpg; *.jpeg; *.gif",
                InitialDirectory = "C:\\"
            };
            bool? rst = openImgDialog.ShowDialog();
            if ((bool)rst)
            {
                // 转交控制器
                new core.control_station.ImgRecognitionControl("imageR", openImgDialog.FileName);
                _pre_mouseLDown = true;
                this.Close();
                return;
            }
             _pre_mouseLDown = false;
        }

        private void Textimg_recog_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _pre_mouseLDown = true;
            Microsoft.Win32.OpenFileDialog openImgDialog = new Microsoft.Win32.OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "文字识别：选择一张有文字内容的图片",
                Multiselect = false,
                Filter = "图片图像|*.png; *.jpg; *.jpeg; *.gif",
                InitialDirectory = "C:\\"
            };
            bool? rst = openImgDialog.ShowDialog();
            if ((bool)rst)
            {
                // 转交控制器
                new core.control_station.ImgRecognitionControl("textimgOCR", openImgDialog.FileName);
                _pre_mouseLDown = true;
                this.Close();
                return;
            }
            _pre_mouseLDown = false;
        }


        private void Image_recog_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void Face_recog_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// 当此窗口变为后台窗口时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecognitionMenu_Deactivated(object sender, EventArgs e)
        {
            /* 若点击了窗口以外区域则关闭。 注: 如果OpenFileDialog执行, 此事件才会接着触发 */
            if (!_pre_mouseLDown)
            {
                this.Close();
            }
        }
    }
}
