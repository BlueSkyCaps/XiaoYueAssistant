using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.control_station.common
{
    /// <summary>
    /// 数据匹配模型 定义了获取答案的匹配顺序
    /// </summary>
    abstract class DataMatchOrder
    {
        abstract public object[] MatchCustomLocalData();
        abstract public object[] MatchRedisDBData();
        abstract public object[] MatchIsTaskCommand();
        abstract public object[] MatchWolframAlpha();
    /*  ......
            */
    }
}
