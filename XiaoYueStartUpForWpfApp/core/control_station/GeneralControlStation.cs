using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaoYueStartUpForWpfApp.core.skins;

namespace XiaoYueStartUpForWpfApp.core.control_station
{
    /// <summary>
    /// 基础的控制器站点类
    /// 用于生成主界面以及副界面, 且为后续的功能提供操作
    /// 副界面不允许重复生成。主界面可以重复生成, 且不会影响到副界面及其子界面
    /// </summary>

    class GeneralControlStation
    {
        // 用于生成的副界面。副界面不需要重复生成, static是必须的
        private static page.secondary.SecondaryInter secondaryInter; 
        private static bool showSecondaryOnce = false; // 判断副界面是否已经打开, 避免重复生成

        // 添加全局Esc按键Hook
        private static readonly common.InterceptEscKey injEscKey = new common.InterceptEscKey();

        public GeneralControlStation(){}

        public static void GetDaytimeAndShow(int enumConv) {
            // 根据时间显示主界面
            // TODO 目前为根据获取的时间直接生成主界面, 后期应添加操作以实现随着时间动态更换主界面
            switch (enumConv)
            {
                case 0:
                    new skins.EarlyMainSkin().Show();
                    break;
                case 1:
                    new skins.DaytimeMainSkin().Show();
                    break;
                case 2:
                    new skins.DuskMainSkin().Show();
                    break;
                case 3:
                    new skins.NightMainSkin().Show();
                    break;
                default:
                    System.Windows.MessageBox.
                        Show("遇到未知错误!" + Environment.NewLine + 
                        "开发人员: unknown error in" + Environment.NewLine + 
                        "core.control_station.GetDaytimeAndShow|core.run.MainSteeringWheel.HourArrangement");
                    // TODO生成错误日志(专属日志类代码)
                    break;
            }
        }

        /// <summary>
        /// 统一显示副界面的方法
        /// </summary>
        /// <param name="skin">要关闭的主界面窗口</param>
        /// <returns></returns>
        public bool SecondaryInterfacePresent(Object skin)
        {
            if (!showSecondaryOnce)
            {
                secondaryInter = new page.secondary.SecondaryInter();
                showSecondaryOnce = true;
            }

            if (skin is EarlyMainSkin earlyMainSkin)
            {
                // 关闭获取的当前主窗口并生成副窗口
                secondaryInter.Show();
                earlyMainSkin.Close();
            }
            else if (skin is DaytimeMainSkin daytimeMainSkin)
            {
                secondaryInter.Show();
                daytimeMainSkin.Close();
            }
            else if (skin is DuskMainSkin duskMainSkin)
            {
                secondaryInter.Show();
                duskMainSkin.Close();
            }
            else if (skin is NightMainSkin nightMainSkin)
            {
                secondaryInter.Show();
                nightMainSkin.Close();
            }
            else {
                return false;
            }
            return true;
        }
    }
}
