using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.merged
{
    /// <summary>
    /// 匹配的答案的位置来源信息
    /// </summary>
    struct Come
    {
        /// <summary>
        /// 来自自定义问答库
        /// </summary>
        static public string CUSTOM_M = "custom matched";
        /// <summary>
        /// 来自数据库Redis
        /// </summary>
        static public string REDIS_M = "redis matched";
        /// <summary>
        /// 当前问题应该是个任务操作命令
        /// </summary>
        static public string TASK = "maybe is task command";
        /// <summary>
        /// Wolfram Alpah匹配的英文答案
        /// </summary>
        static public string WolframAlpah_EN = "WolframAlpah matched use en";
        /// <summary>
        /// WolframAlpah匹配的答案, 但翻译为中文
        /// </summary>
        static public string WolframAlpah_ZH = "WolframAlpah matched use zh";
    }
}
