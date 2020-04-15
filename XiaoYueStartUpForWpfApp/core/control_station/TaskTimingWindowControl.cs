using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace XiaoYueStartUpForWpfApp.core.control_station
{
    /// <summary>
    /// TaskTimingWindow(定时任务)类绑定的控制类
    /// 生成以及管理定时任务线程。为此窗口下用户的数据做处理
    /// </summary>
    /*
     * 定时任务说明: 用户最多只能设置三个定时任务。退出主程序后, 将不保存数据且取消所有定时任务的线程 
     * 
     * TODO(保存定时任务的数据于Json, 这是因为需要判断用户是否设置了定时任务, 并且重新打开程序后恢复数据)
     * TODO(使用Json用于read, modify json对象)
     */

    class TaskTimingWindowControl : GeneralControlStation
    {
        /// <summary>
        /// 一个字典集合, string用来绑定Task以管理定时任务线程
        /// </summary>
        private static Dictionary<string, Task> taskDict = new Dictionary<string, Task>();

        /// <summary>
        /// 一个字典集合, string用来绑定CancellationTokenSource关联的CancellationToken
        /// CancellationToken传递取消标志给指定线程
        /// </summary>
        private static Dictionary<string, CancellationTokenSource> ctsDict = new Dictionary<string, CancellationTokenSource>();

        // private Action<object> taskAction; // Task所需的委托

        /// <summary>
        /// 生成一个定时任务线程
        /// </summary>
        /// <param name="scbutton">当前指定的Button源, 用来绑定线程值</param>
        /// <param name="taskTime">当前指定任务的触发时间</param>
        /// <param name="comboboxText">当前指定的Combobox选定项</param>
        /// <param name="title">当前定时任务的名字</param>
        /// <param name="remarks">当前定时任务的备注</param>
        /// <returns></returns>
        public bool MakeTaskThread(Button scbutton, string taskTime, string comboboxText, string title, string remarks)
        {
            try
            {
                string[] type = comboboxText.Split(new char[] {'|'});
                if (type[1] == "闹钟")
                {
                    CancellationTokenSource tokenSource = new CancellationTokenSource(); // 创建取消Task记号的发送源
                    ctsDict.Add(scbutton.Name, tokenSource);
                    Task task = Task.Run( () => AlarmThread(taskTime, scbutton, tokenSource.Token)); // 启动任务线程
                    taskDict.Add(scbutton.Name, task);
                }
                else if (type[1] == "关机")
                {
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    ctsDict.Add(scbutton.Name, tokenSource);
                    Task task = Task.Run(() => ShutdownThread(taskTime, scbutton, tokenSource.Token));
                    taskDict.Add(scbutton.Name, task);
                }
                else if (type[1] == "锁屏")
                {
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    ctsDict.Add(scbutton.Name, tokenSource);
                    Task task = Task.Run(() => LockScreenThread(taskTime, scbutton, tokenSource.Token));
                    taskDict.Add(scbutton.Name, task);
                }
                else
                {
                    // 如果任务是定时打开
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    ctsDict.Add(scbutton.Name, tokenSource);
                    Task task = Task.Run(() => OpenExeThread(comboboxText, scbutton, tokenSource.Token));
                    taskDict.Add(scbutton.Name, task);
                }

            }
            catch (ArgumentNullException)
            {
                Console.WriteLine(Console.Error);
                return false;

            }
            catch (ArgumentException)
            {
                Console.WriteLine(Console.Error);
                return false;
            }
            catch (Exception)
            {
                Console.WriteLine(Console.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 撤销当前的定时任务。 发送取消通知, 移除绑定对象, 释放所用资源
        /// </summary>
        /// <param name="scbutton">指定的当前任务的撤销按钮(同开始按钮)</param>
        /// <returns></returns>
        public bool CancelTheTaskThread(Button scbutton) {
            try
            {
                ctsDict[scbutton.Name].Cancel(); // 发送取消请求给指定的Task
                ctsDict[scbutton.Name].Dispose();
                ctsDict.Remove(scbutton.Name);
                System.Threading.Thread.Sleep(500);
                if (taskDict[scbutton.Name].IsCompleted || taskDict[scbutton.Name].IsCanceled)
                {
                    taskDict[scbutton.Name].Dispose(); //释放任务
                    taskDict.Remove(scbutton.Name);
                    Console.WriteLine("task canceled");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                Console.WriteLine(Console.Error);
                return false;
            }
        }

        /// <summary>
        /// Action AlarmThread定时执行的提醒内容
        /// </summary>
        /// <param name="time">Task传递的触发时间</param>
        /// <param name="scbutton">用于清理的按钮</param>
        /// <param name="token">CancellationTokenSource的取消标志</param>
        private void AlarmThread(object time, Button scbutton, CancellationToken token)
        {

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true) {
                if (token.IsCancellationRequested)
                {
                    return; // !made the thread completed.
                }
                
                if (stopWatch.Elapsed >= TimeSpan.Parse((string)time)) {
                    void AutoClearDispose()
                    {
                        // 自动触发后清理以及更新
                        ctsDict.Remove(scbutton.Name);
                        taskDict.Remove(scbutton.Name);
                        scbutton.Content = "开始";
                    }
                    scbutton.Dispatcher.Invoke(() => AutoClearDispose());
                    /* 下行代码应在前面之后, 否则AutoClearDispose会等待MessageBox导致更新不及时 */
                    System.Windows.Forms.MessageBox.Show(time+"秒闹钟", "提示", System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
            }
        }

        /// <summary>
        /// Action OpenExeThread定时执行的打开操作
        /// </summary>
        /// <param name="comboboxText">Task传递的时间和打开文件路径</param>
        private void OpenExeThread(object comboboxText, Button scbutton, CancellationToken token)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string time = comboboxText.ToString().Split(new char[] { '|' })[0];
            string fpath = comboboxText.ToString().Split(new char[] { '|' })[2];
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                if (stopWatch.Elapsed >= TimeSpan.Parse(time))
                {
                    void AutoClearDispose()
                    {
                        ctsDict.Remove(scbutton.Name);
                        taskDict.Remove(scbutton.Name);
                        scbutton.Content = "开始";
                    }
                    scbutton.Dispatcher.Invoke(() => AutoClearDispose());
                    System.Windows.Forms.MessageBox.Show("打开到"+ comboboxText.ToString());
                    return;
                }
            }
        }

        /// <summary>
        /// Action ShutdownThread定时执行的关机命令
        /// </summary>
        /// <param name="time">Task传递的触发时间</param>
        private void ShutdownThread(object time, Button scbutton, CancellationToken token)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                if (stopWatch.Elapsed >= TimeSpan.Parse((string)time))
                {
                    void AutoClearDispose()
                    {
                        ctsDict.Remove(scbutton.Name);
                        taskDict.Remove(scbutton.Name);
                        scbutton.Content = "开始";
                    }
                    scbutton.Dispatcher.Invoke(() => AutoClearDispose());
                    System.Windows.Forms.MessageBox.Show("关机到"+time.ToString());
                    return;
                }
            }
        }

        /// <summary>
        /// Action LockScreenThread定时执行的锁屏命令
        /// </summary>
        /// <param name="time">Task传递的触发时间</param>
        private void LockScreenThread(object time, Button scbutton, CancellationToken token)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                if (stopWatch.Elapsed >= TimeSpan.Parse((string)time))
                {
                    void AutoClearDispose()
                    {
                        ctsDict.Remove(scbutton.Name);
                        taskDict.Remove(scbutton.Name);
                        scbutton.Content = "开始";
                    }
                    scbutton.Dispatcher.Invoke(() => AutoClearDispose());
                    System.Windows.Forms.MessageBox.Show("锁屏到"+time.ToString());
                    return;
                }
            }
        }

        /// <summary>
        /// Action CancellationToken注册的取消指定线程的委托
        /// !!! 此方式对于并行任务是很有效的, 但不使用
        /// </summary>
        private void CancelAlarmThreadA(CancellationToken cToken) {}
        /// <summary>
        /// Action CancellationToken注册的取消指定线程的委托
        /// </summary>
        private void CancelOpenExeThreadA(CancellationToken cToken) {}
        /// <summary>
        /// Action CancellationToken注册的取消指定线程的委托
        /// </summary>
        private void CancelShutdownThreadA(CancellationToken cToken) {}
        /// <summary>
        /// Action CancellationToken注册的取消指定线程的委托
        /// </summary>
        private void CancelLockScreenThreadA(CancellationToken cToken) {}

        /// <summary>
        /// 此函数专门用于判断定时任务是否设置过, 每当用户重新打开主程序, 此函数总会执行
        /// 若设置过, 将会重绘回设置过的界面, 并且重启已有任务
        /// </summary>
        // TODO (目前不需要)
        public static bool IsHaveTaskData()
        {
            // TODO
            using (StreamReader streamReader = File.OpenText("parameters/save/timing-task-data.json"))
            {
                string jsonText = streamReader.ReadToEnd();
                JObject jobj = JObject.Parse(jsonText);
                if (jobj.SelectToken("first.flag").ToString() == "no"
                    && jobj.SelectToken("second.flag").ToString() == "no"
                        && jobj.SelectToken("third.flag").ToString() == "no")
                    return false;
            }

            StreamReader streamReaderT = File.OpenText("parameters/save/timing-task-data.json");
            string jsonTextT = streamReaderT.ReadToEnd();
            streamReaderT.Close();

            JObject jobjT = JObject.Parse(jsonTextT);
            JObject f = (JObject)jobjT["first"];
            f["flag"] = "yes";
            Console.WriteLine(jobjT.ToString());
            using (FileStream fw = File.OpenWrite("parameters/save/timing-task-data.json"))
            {
                byte[] b = new UTF8Encoding(true).GetBytes(jobjT.ToString());
                fw.Write(b, 0, b.Length);
            }
            return true;
        }
        
        public static void ReloadTaskTimingData()
        {
            // TODO
        }

        public static void SaveTaskTimingData()
        {
            // TODO
        }

        private void ReadTaskTimingData()
        {
            // TODO
        }
    }
}
