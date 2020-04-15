using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.control_station
{
    /// <summary>
    /// 自定义问答(魔法训练)控制器。将自定义数据添加至本地文件以及更新等
    /// </summary>
    /// TODO 允许用户更新或删除之前添加过的每个自定义问答
    class CustomQAControl : GeneralControlStation
    {
        /// <summary>
        /// 将当前的自定义问答添加至本地文件
        /// 此控制器操控data.UserCustomData
        /// </summary>
        /// <param name="customQ">当前问题内容</param>
        /// <param name="customA">问题的答案</param>
        public CustomQAControl(string customQ, string customA)
        {
            AddOneToCustom(customQ, customA);
        }

        private void AddOneToCustom(string customQ, string customA)
        {
            // 统一utf8编码
            UTF8Encoding utf8Encoding = new UTF8Encoding(true);
            byte[] utf8Bytes = utf8Encoding.GetBytes(customQ);
            string customQUtf8 = utf8Encoding.GetString(utf8Bytes);

            utf8Bytes = utf8Encoding.GetBytes(customA);
            string customAUtf8 = utf8Encoding.GetString(utf8Bytes);

            string dataPath = "parameters/settings/server/custom-data/user-custom-data.json";
            JObject qaJsonObject = new data.UserCustomData().ReadJsonCustomData(dataPath);
            JToken qasToken = qaJsonObject.SelectToken("qas");
            // 避免键存在空格导致解析错误问题
            if (qasToken.SelectToken("['" + customQUtf8 + "']") != null)
            {
                System.Windows.Forms.MessageBox.Show("已经存在一模一样的自定义问题了！换个问法吧。", "提示");
                return;
            }
            // 添加当前一个问答至json
            bool isSuccess = new data.UserCustomData().AddJsonCustomData(ref qaJsonObject, ref qasToken, customQUtf8, customAUtf8, dataPath);
            if (!isSuccess)
            {
                System.Windows.Forms.MessageBox.Show("出错了..不好意思！", "提示");
                return;
            }
            // 总数量+1
            JToken countToken = qaJsonObject.SelectToken("count");
            countToken.Replace((int)countToken + 1);
            using (FileStream jsonFile = File.OpenWrite(dataPath))
            {
                byte[] newData = utf8Encoding.GetBytes(qaJsonObject.ToString());
                jsonFile.Write(newData, 0, newData.Length);
            }
            System.Windows.Forms.MessageBox.Show("成功训练了一个技能！", "XiaoYue");
        }
    }
}
