using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace XiaoYueStartUpForWpfApp.core.control_station
{
    /// <summary>
    /// 专门用于主界面进行语音命令交互时的控制类
    /// 语音识别开始产生时、语音识别结束时等逻辑控制、界面显示
    /// </summary>
    class SpaceKeySpeechControl: GeneralControlStation
    {
        // MakeTansparentScreen()函数返回的窗体对象
        private static Window mask;

        // 当此次语音识别超时(10s), 自动关闭当前任务
        private static System.Timers.Timer overSpeekingTimer;

        // 当前的主界面对象
        private static Window skin;

        // 进度条窗口
        private static page.other.ProgressRunning progressRunning;

        // 当前主界面位置
        private static double locationX = .0, locationY = .0;

        // 响应的应答界面
        private static page.other.SpeechTextDisplay textDisplay = new page.other.SpeechTextDisplay("", "");

        /* 用于提示说话超时的窗口, 此窗口由指定的线程控制。无法避免的概率是用户有可能多次超时。
         * 为了线程的合理操控, 因此需要多个对象。虽然新对象产生时上一个对象就已经回收了 */
        private Window timeoutAlert = new Window();

        // 是否说话超时的标志。此对象应该Static, 因为它只需要拿来判断当前窗口是否超时
        private static bool _isOverSpeech = false;

        // 是否与自己服务器进行过通信
        private static bool _isGotKey = false;

        // 当前语音的语义可能有关任务操作
        private bool _isTaskCommand = false;

        private static string APP_ID;
        private static string API_KEY;
        private static string SECRET_KEY;

        private Baidu.Aip.Speech.Asr asrClient;

        /// <summary>
        /// Call默认麦克风-媒体接口, 以发送需要的命令广播
        /// </summary>
        /// <param name="lpstrCommand">命令字符串</param>
        /// <param name="lpstrReturnString">返回信息</param>
        /// <param name="uReturnLength"></param>
        /// <param name="hwndCallback"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int RecordWaveSpeech(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        static SpaceKeySpeechControl() {}
        public void SpaceKeySpeechControlReceive(string eventName, Window gskin)
        {
            skin = gskin;
            if (eventName == "KeyDown")
            {
                SpaceKeySpeechControl.textDisplay.Close(); // 关闭当前textDisplay, 如果存在的话
                locationX = gskin.Left;
                locationY = gskin.Top;
                overSpeekingTimer = new System.Timers.Timer();
                overSpeekingTimer.AutoReset = false;
                overSpeekingTimer.Interval = 10000;
                overSpeekingTimer.Enabled = true;
                overSpeekingTimer.Elapsed += OverSpeekingTimer_Elapsed;
                // 调用方法-显示遮罩层
                mask = common.CommonControlFunc.MakeTansparentScreen();
                skin.Left = .0;
                skin.Top = .0;
                // 将主界面激活到前台, 确保使其能够接收到键盘按键
                skin.Activate();
                skin.Focus();
                Cursor.Hide();
                RecordOnceSpeech(); // 开始录下用户的麦克风
                overSpeekingTimer.Start();
            }
            else if(eventName == "KeyUp"){
                Cursor.Show();
                skin.Focus();
                if (SpaceKeySpeechControl._isOverSpeech)
                {
                    // 如果标志True, 此块之外的代码在OverSpeekingTimer_Elapsed中已执行
                    SpaceKeySpeechControl._isOverSpeech = false;
                    return;
                }
                // 当用户释放了Space Key, 释放用于遮罩的窗体对象。处理录音数据。
                mask.Close();
                skin.Left = locationX;
                skin.Top = locationY;
                overSpeekingTimer.Enabled = false;
                overSpeekingTimer.Stop();
                overSpeekingTimer.Close();
                RecordOnceSpeechDone();
            }
        }

        /// <summary>
        /// 用户仍处于语音输入但超时, 执行此事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OverSpeekingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SpaceKeySpeechControl._isOverSpeech = true;
            RecordBreak();
            Action closeingAction = () =>{
                mask.Close();
                skin.Left = locationX;
                skin.Top = locationY;
                Cursor.Show();
            };
            // 此事件线程非控件原线程
            mask.Dispatcher.Invoke(closeingAction);

            ThreadStart alertDelegate = () =>
            {
                timeoutAlert.Dispatcher.Invoke(() => {
                    timeoutAlert = new Window
                    {
                        ShowInTaskbar = false,
                        Top = .0,
                        Left = .0,
                        Focusable = false,
                        SizeToContent = SizeToContent.WidthAndHeight,
                        // Topmost = true,
                        WindowStyle = WindowStyle.None,
                        ResizeMode = ResizeMode.NoResize
                    };
                    System.Windows.Controls.Label textLabel = new System.Windows.Controls.Label
                    {
                        Width = 250,
                        Height = 50,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Background = System.Windows.Media.Brushes.BlueViolet,
                        Foreground = System.Windows.Media.Brushes.White,
                        Content = "OMG!说话太久了..",
                        FontSize = 25,
                        FontWeight = System.Windows.FontWeights.ExtraBlack,
                        FontStyle = System.Windows.FontStyles.Normal,
                    };
                    System.Windows.Controls.StackPanel stackPanel = new System.Windows.Controls.StackPanel();
                    stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    stackPanel.VerticalAlignment = VerticalAlignment.Center;
                    stackPanel.Children.Add(textLabel);
                    timeoutAlert.Content = stackPanel;
                    timeoutAlert.Show();
                    // 确保焦点在主界面。此代码应位于此处
                    skin.Dispatcher.Invoke(() => { skin.Activate(); skin.Focus(); });
                });
                // 在当前ThreadStart线程上运行1.5s以提示用户
                System.Threading.Thread.Sleep(1500);
                timeoutAlert.Dispatcher.Invoke(() =>{
                    timeoutAlert.Close();
                });
                return;
            };
            // 创建提醒超时的控制线程
            Thread alertT = new Thread(alertDelegate);
            alertT.Start();

            overSpeekingTimer.Enabled = false;
            overSpeekingTimer.Stop();
            overSpeekingTimer.Close();
        }

        /// <summary>
        /// 记录麦克风输入
        /// </summary>
        private void RecordOnceSpeech()
        {
            int openR = RecordWaveSpeech("open new type waveaudio alias df-record-wave", "", 0, 0);
            int recordR = RecordWaveSpeech("record df-record-wave", "", 0, 0);
            Console.WriteLine("record");
        }

        /// <summary>
        /// 完成当前的麦克风输入以及后续操作
        /// </summary>
        private void RecordOnceSpeechDone()
        {
            int saveR = RecordWaveSpeech("save df-record-wave " + AppDomain.CurrentDomain.BaseDirectory + "parameters\\save\\ONCE-SPEECH.wav", "", 0, 0);
            int closeR = RecordWaveSpeech("close df-record-wave", "", 0, 0);
            Console.WriteLine("done");
            // 显示进度条提示 线程异步执行命令处理
            progressRunning = new page.other.ProgressRunning();
            progressRunning.Show();
            Thread backDataAnalyze = new Thread(new ThreadStart(HandelTheSpeechData));
            backDataAnalyze.SetApartmentState(ApartmentState.STA);
            backDataAnalyze.IsBackground = true;
            backDataAnalyze.Start();
        }

        /// <summary>
        /// 停止当前的麦克风当不希望的操作发生时
        /// </summary>
        /// 目前用不到此方法
        private void RecordBreak()
        {
            int stopR = RecordWaveSpeech("stop df-record-wave", "", 0, 0);
            int closeR = RecordWaveSpeech("close df-record-wave", "", 0, 0);
            Console.WriteLine("break");
        }
        /// <summary>
        /// 处理麦克风数据并响应
        /// </summary>
        private void HandelTheSpeechData()
        {
            // 启动ffmpeg进程, 编码指定格式的语音文件-pcm-采样率16k
            using (Process ffmpeg = new Process())
            {
                ffmpeg.StartInfo.CreateNoWindow = true;
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.RedirectStandardOutput = true;
                ffmpeg.StartInfo.FileName = "tools\\ffmpeg-4.2.2-win32-shared\\bin\\ffmpeg.exe";
                ffmpeg.StartInfo.Arguments = " -y -i " + AppDomain.CurrentDomain.BaseDirectory +
                    "parameters\\save\\ONCE-SPEECH.wav -acodec pcm_s16le -f s16le -ac 1 -ar 16000 " +
                    AppDomain.CurrentDomain.BaseDirectory + "parameters\\save\\ONCE-SPEECH.pcm";
                ffmpeg.Start();
                Console.WriteLine("wait ffmpeg");
                ffmpeg.WaitForExit(10000);
            }
            byte[] onceSpeechData = File.ReadAllBytes("parameters\\save\\ONCE-SPEECH.pcm");

            // 第一次与自身服务器通信? 获取密钥
            if (!_isGotKey)
            {
                Console.WriteLine("first get");
                string ownerUri = "http://www.coollsx.com/";
                string toUri = ownerUri + "api-station/direct/withkey/bd";
                string queryUri = toUri + "?code=c5e8962fb24a491670e1e79a8702439b&type=spd";
                HttpWebRequest httpWebRequest = WebRequest.CreateHttp(queryUri);
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 10000;
                httpWebRequest.AllowAutoRedirect = true;
                StreamReader streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), Encoding.UTF8);
                JObject jObject = JObject.Parse(streamReader.ReadToEnd());
                streamReader.Close();
                streamReader.Dispose();
                SpaceKeySpeechControl.APP_ID = (string)jObject.GetValue("APP_ID");
                SpaceKeySpeechControl.API_KEY = (string)jObject.GetValue("API_KEY");
                SpaceKeySpeechControl.SECRET_KEY = (string)jObject.GetValue("SECRET_KEY");
                _isGotKey = true;
            }
            // 初始化语音识别实例
            asrClient = new Baidu.Aip.Speech.Asr(APP_ID, API_KEY, SECRET_KEY);
            var options = new Dictionary<string, object>
                {
                    {"dev_pid", 1537} // 中英文代号-识别中英文
                };

            asrClient.Timeout = 10000;
            JObject result;
            try
            {
                // 获取识别结果
                result = asrClient.Recognize(onceSpeechData, "pcm", 16000, options);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("糟糕！网络连接错误..", "XiaoYue", MessageBoxButtons.OK);
                return;
            }

            bool isHaveResult = result.ContainsKey("result");
            string yourText, XYFinalAnwser;
            if (isHaveResult)
            {
                Console.WriteLine(result);
                string speechText = (string)result["result"].First;
                Console.WriteLine(speechText);
                // 要么无result, 通常百度不会返回空结果。此处应属多余
                if (String.IsNullOrWhiteSpace(speechText))
                {
                    yourText = "...";
                    XYFinalAnwser = "空空如也？老娘啥都听不到！";
                }else if (speechText == "我不知道。") // 有声音内容但过于微弱 百度返回类似           
                {
                    yourText = "...";
                    XYFinalAnwser = "嗯？你似乎啥也没说..";
                }else
                {
                    // 执行数据模型, 以匹配有效的答案, 并显示应答窗口
                    ResultThinkTheSpeechControl thinkTheSpeechControl = new ResultThinkTheSpeechControl(speechText);
                    if (thinkTheSpeechControl.SpeechAnwser != null)
                    {
                        // 如果数据模型分析出可能是个任务操作命令 todo
                        if (thinkTheSpeechControl.SpeechAnwser == merged.Come.TASK)
                        {
                            //this._isTaskCommand = true;
                            //new common.SynthesizerOnceSpeak("好的，..."); // 语音回应, “好的。”?
                            //using (Process openAppProcess = new Process())
                            //{

                            //}
                        }
                        yourText = speechText;
                        XYFinalAnwser = thinkTheSpeechControl.SpeechAnwser;
                    }
                    else
                    {
                        yourText = speechText;
                        XYFinalAnwser = "抱歉，我暂时不明白..";
                    }
                }
            }
            else
            {
                yourText = "...";
                XYFinalAnwser = "太快了吧，老娘啥都听不到！";
            }
            Action parentThreadAction = () =>
            {
                // 若是任务操作命令, 不必显示相应窗口
                if (!_isTaskCommand)
                {
                    // textDisplay必须重新生成, 因为当前的它在space按下时已经被关闭
                    SpaceKeySpeechControl.textDisplay = new page.other.SpeechTextDisplay(yourText, XYFinalAnwser);
                    SpaceKeySpeechControl.textDisplay.Show(); // 其在SpaceKeySpeechControl上生成, 非当前backDataAnalyze线程
                    new common.SynthesizerOnceSpeak(XYFinalAnwser); // 语言合成说出答案
                }
                progressRunning.Close();
                skin.Focus();
            };
            // 在其主线程上进行操作
            skin.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, parentThreadAction);
            return;
        }
    }
}
