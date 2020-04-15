using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.skins.common
{
    interface ISkinsSpaceKey
    {
        /// <summary>
        /// 所有主界面都应实现的接口, 此方法接听语音输入的按键
        /// </summary>
        void AddSpaceKeyListener();
    }
}
