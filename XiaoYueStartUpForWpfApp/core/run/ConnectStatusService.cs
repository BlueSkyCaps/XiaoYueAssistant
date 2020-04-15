using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace XiaoYueStartUpForWpfApp
{
    class ConnectStatusService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        // 用于立即获取用户环境情况的Timer, 不应该使用WPF线程中的Windows.Threading.DispatcherTimer
        private static Timer timer;
        // 用于测试GET请求结果的访问网址
        public static String _msUri = "http://www.aliyun.com/";
        // 因特网连接与否状态码
        public static bool _isConnected = true;
        public static String statusCodeFinshedMessage = null;

        public ConnectStatusService()
        {
            // 立即启动InitializePreprocessing_ElapsedAsync
            timer = new Timer() { Interval = 1, AutoReset = false, Enabled = true };
            timer.Elapsed += InitializePreprocessing_ElapsedAsync;
            timer.Start();
        }

        private async void InitializePreprocessing_ElapsedAsync(object source, ElapsedEventArgs e)
        {
            Task<String> statusCodeRunning = PreprocessingAsync();

            try
            {
                // 当此操作结束时更新响应正文
                String statusCodeFinshed = await statusCodeRunning;
                statusCodeFinshedMessage = statusCodeFinshed;
            }
            catch (Exception)
            {
                _isConnected = false;
            }
        }

        private async Task<String> PreprocessingAsync()
        {
            /// <summary>
            /// 此为异步操作方法
            /// 用于检测当前的Internet连接状态(以GetStringAsync的方式收获响应)
            /// </summary>
            try
            {
                String message = await httpClient.GetStringAsync(_msUri);
                return message;
            }
            catch (Exception)
            {
                Console.WriteLine("catch!");
                // 当发生HttpRequestException时会捕获, 更新状态码为False
                _isConnected = false;
                throw;
            }
        }
    }
}
