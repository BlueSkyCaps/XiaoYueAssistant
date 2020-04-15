using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.data
{
    /// <summary>
    /// 用于操作本地用户自定义数据
    /// </summary>
    class UserCustomData : ICustomData
    {

        /// <summary>
        /// 读取指定路径的自定义问答json数据
        /// </summary>
        /// <param name="dataPath">自定义问答数据文件路径</param>
        /// <returns>JObject: 用户自定义问答数据</returns>
        public JObject ReadJsonCustomData(string dataPath)
        {
            string str;
            JObject qaJsonData;
            using (StreamReader streamReader = File.OpenText(dataPath))
            {
                str = streamReader.ReadToEnd();
            }
            qaJsonData = JObject.Parse(str);
            return qaJsonData;
        }

        /// <summary>
        /// 添加一个当前的自定义问答数据
        /// </summary>
        /// <param name="qasToken">CustomQAControl传递的当前JToken对象, 用于追加</param>
        /// <param name="customQUtf8">要添加的utf-8编码的问题</param>
        /// <param name="customAUtf8">要添加的utf-8编码的答案</param>
        /// <param name="dataPath">自定义问答数据文件路径</param>
        /// <returns>true or false</returns>
        public bool AddJsonCustomData(ref JObject qaJsonObject, ref JToken qasToken, string customQUtf8, string customAUtf8, string dataPath)
        {
            try
            {
                qasToken[customQUtf8] = customAUtf8;
                // utf8编码
                UTF8Encoding utf8Encoding = new UTF8Encoding(true);
                using (FileStream jsonFile = File.OpenWrite(dataPath))
                {
                    byte[] newData = utf8Encoding.GetBytes(qaJsonObject.ToString());
                    jsonFile.Write(newData, 0, newData.Length);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //todo 编辑存在的自定义问答
        public bool UpdateJsonCustomData()
        {
            throw new NotImplementedException();
        }
        
        //todo 删除一个存在的自定义问答
        public bool DeleteJsonCustomData()
        {
            throw new NotImplementedException();
        }
    }
}
