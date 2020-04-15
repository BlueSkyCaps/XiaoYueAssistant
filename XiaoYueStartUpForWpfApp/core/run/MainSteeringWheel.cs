using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using XiaoYueStartUpForWpfApp.core.run;

namespace XiaoYueStartUpForWpfApp.core.run
{
    class MainSteeringWheel
    {
        /// <summary>
        /// 此为项目控制主入口
        /// 它没有控制的主要逻辑, 它只是为控制提供单一方向
        /// 控制逻辑的唯一门户
        /// </summary>
        public MainSteeringWheel()
        {

            //new ConnectStatusService();

            //// 将主调用方挂起。这种异步等待是不应该的
            //System.Threading.Thread.Sleep(3000);
            
            //// XiaoYue程序需要因特网, 因此网络连接需求是必须的
            //// if _isConnected is False: 用户未连接互联网
            //if (!ConnectStatusService._isConnected)
            //{
            //    // 当互联网需求不可得时, 创建未联网的提示界面
            //    Console.WriteLine("用户未联网！");
            //    return;
            //}
            
            //Console.WriteLine(ConnectStatusService._isConnected);
            //Console.WriteLine(ConnectStatusService.statusCodeFinshedMessage);
            HourArrangement();

        }

        private void HourArrangement()
        {
            /// <summary>
            /// 根据四类时间(清晨Early|白天Daytime|黄昏Dusk|夜晚Night)初始化不同的Xaml Page.
            /// 
            /// 清晨:5:00-7:59:59
            /// 白天:8:00-16:59:59
            /// 黄昏:17:00-18:59:59
            /// 夜晚:19:00-4:59:59
            /// </summary>

            // 获取的Hour是本机时间, 因为用户系统时间可能有误, 因此后期需要TODO服务器端的网络时间
            int curTimeHour = DateTime.Now.Hour;
            if (curTimeHour >= 5 && curTimeHour < 8)
            {
                control_station.GeneralControlStation.GetDaytimeAndShow((int)merged.Day.Early);
            }
            else if (curTimeHour >= 8 && curTimeHour < 17)
            {
                control_station.GeneralControlStation.GetDaytimeAndShow((int)merged.Day.Daytime);
            }
            else if(curTimeHour >= 17 && curTimeHour < 19)
            {
                control_station.GeneralControlStation.GetDaytimeAndShow((int)merged.Day.Dusk);
            }
            else
            {
                control_station.GeneralControlStation.GetDaytimeAndShow((int)merged.Day.Night);
            }
        }
    }
}
