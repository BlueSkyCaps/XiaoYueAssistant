using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.control_station
{
    /// <summary>
    /// 语音识别之后, 接收语音命令, 执行数据模型, 匹配有效答案。
    /// 此控制器与副界面文字命令模块无关
    /// </summary>
    class ResultThinkTheSpeechControl : common.DataMatchOrder
    {
        public string SpeechAnwser { get; set; }
        public string SpeechQuestion { get; set; }
        private string customDataPath = "parameters/settings/server/custom-data/user-custom-data.json";
        /// <summary>
        /// 这是WolframAlpha无匹配的回应
        /// </summary>
        private readonly string _waNoneAnswer = "No short answer available";
        /// <summary>
        /// 这是WplframAlpha无法理解问题的回应
        /// </summary>
        private readonly string _waNotUnderstand = "Wolfram|Alpha did not understand your input";
        private StreamReader streamReader;
        private HttpWebRequest request;
        private HttpWebResponse response;

        public ResultThinkTheSpeechControl(string speechQuestion)
        {
            /*
             * 去除问题中以下的标点符号(自定义问答库只允许中间空格以后期需求?兼容英文问答)
             * 存在多余符号的问题目前WplframAlpha API会有影响
             */
            String[] splitStrs = speechQuestion.Trim().Split(new char[] {
                ',', '.', '/', ';', '\'', '?', '"', ':', '!', '-', '+',
                '=', '*', '/', '@', '#', '$', '%', '^', '&', '(', ')',
                '~', '|', '\\', '`', '。', '，', '！', '？', '“', '”',
                '：', '；', '‘', '’', '【', '】', '{', '}', '、', '￥',
                '_', '<', '>', '《', '》', '·', '…', '[', ']', '（', '）',
                '—' }
           );
            IEnumerable<string> ienumerable = splitStrs.Take(splitStrs.Length);
            IEnumerator<string> enumerator = ienumerable.GetEnumerator();
            string questionNoPunc = "";
            while (enumerator.MoveNext())
            {
                questionNoPunc += enumerator.Current;
            }
            enumerator.Dispose();
            this.SpeechQuestion = questionNoPunc;
            Console.WriteLine("问题(消除标点):" + this.SpeechQuestion);
            // 开始匹配
            object[] rsts =  MatchCustomLocalData();
            // 如果false, 无结果。否则, 调用方可以访问SpeechAnwser获取结果
            if (!(bool)rsts[0])
            {
                this.SpeechAnwser = null;
                return;
            }
            // 如果分析问题包含任务操作的相关语义
            if ((string)rsts[2] == core.merged.Come.TASK)
            {
                //TODO
                this.SpeechAnwser = core.merged.Come.TASK; //可以交由调用方处理
                return;
            }
            return;
        }

        /// <summary>
        /// 查询本地自定义问答库是否存在匹配
        /// </summary>
        /// <returns></returns>
        public override object[] MatchCustomLocalData()
        {

            string data = File.ReadAllText(customDataPath, Encoding.UTF8);
            JObject jObject = JObject.Parse(data);
            if (jObject["qas"][this.SpeechQuestion] != null)
            {
                this.SpeechAnwser = jObject["qas"][this.SpeechQuestion].ToString();
                return new object[] { true, this.SpeechAnwser, core.merged.Come.CUSTOM_M };
            }
            return MatchRedisDBData();
        }

        /// <summary>
        /// 查询远程数据库是否存在匹配
        /// </summary>
        /// <returns></returns>
        public override object[] MatchRedisDBData()
        {
            try
            {
                RedisDbControl redisDbControl = new RedisDbControl(this.SpeechQuestion);
                object value = redisDbControl.GetAnswer();
                if (!(value is false))
                {
                    this.SpeechAnwser = value.ToString(); //不允许强制转换RdisValue类型
                    return new object[] { true, this.SpeechAnwser, core.merged.Come.REDIS_M };
                }
            }
            catch (Exception)
            {
                return MatchIsTaskCommand();
            }
            return MatchIsTaskCommand();
        }

        /// <summary>
        /// 判断问题, 分析语义是否是任务命令相关
        /// TODO
        /// </summary>
        /// <returns></returns>
        public override object[] MatchIsTaskCommand()
        {
            //return new object[] { true, core.merged.Come.TASK, core.merged.Come.TASK };
            return MatchWolframAlpha();
        }

        /// <summary>
        /// 查询WolframAlpha API
        /// </summary>
        /// <returns></returns>
        public override object[] MatchWolframAlpha()
        {
            try
            {
                string englishQ = TranslationRing(SpeechQuestion, "en");
                Console.WriteLine("english anwser:" + englishQ);
                if (englishQ == null)
                    return new object[] { false };
                string englishA = GetWolfamAlphaAnwser(englishQ);
                Console.WriteLine("english anwser:" + englishA);
                if (englishA == _waNoneAnswer || englishA == _waNotUnderstand)
                    return new object[] { false };
                string chineseA = TranslationRing(englishA, "zh");
                if (chineseA == null)
                {
                    // 在翻译答案为中文的过程中出现问题, 可以用英语答案
                    this.SpeechAnwser = englishA;
                    return new object[] { true, this.SpeechAnwser, core.merged.Come.WolframAlpah_EN };
                }
                this.SpeechAnwser = chineseA + englishA;
                return new object[] { true, this.SpeechAnwser, core.merged.Come.WolframAlpah_ZH };
            }
            catch (Exception)
            {
                return new object[] { false };
            }
        }

        /// <summary>
        /// 调用自身服务器, 翻译文字, WolfmanAlpha需要英文。原文问题转英文, 英文答案转中文 
        /// </summary>
        /// <param name="originalText">需要翻译的文本</param>
        /// <param name="to">目标语言</param>
        /// <returns></returns>
        private string TranslationRing(string originalText, string to)
        {
            Encoding encoding = Encoding.UTF8;
            /* 
            下行Url访问自己的服务器获取翻译结果，你必须确保服务器已开启。需要处理返回的结果，并在此处直接接收，
            相关服务器脚本已经给出，参见根目录README.md
            当然，如果可以，你可以直接在此处处理API返回的结果，而不是用本人给出的脚本。查询接口文档，访问http://api.fanyi.baidu.com/product/11
            */
            string ownerUri = "http://localhost:80/fy";
            // string ownerUri = "http://www.coollsx.com/api-station/transfer/.../text-tr/general";
            string qUri = ownerUri + "/index.php";
            this.request = (HttpWebRequest)WebRequest.Create(qUri);
            this.request.Method = "POST";
            this.request.KeepAlive = true;
            this.request.AllowAutoRedirect = false;
            this.request.Timeout = 15000;
            this.request.ContentType = "application/x-www-form-urlencoded";
            // 参数code为自己配置的服务器授权密码，你可以在相关服务器脚本中修改，并在此处更新。
            string fields = "code=" + "c5e8962fb24a491670e1e79a8702439b" + "&original=" + WebUtility.UrlEncode(originalText) + "&to=" + to;
            byte[] fields_bytes = encoding.GetBytes(fields);
            this.request.GetRequestStream().Write(fields_bytes, 0, fields_bytes.Length);
            this.response = (HttpWebResponse)request.GetResponse();
            this.streamReader = new System.IO.StreamReader(this.response.GetResponseStream(), Encoding.UTF8);
            string rst = streamReader.ReadToEnd();
            this.response.Close();
            this.response.Dispose();
            this.streamReader.Close();
            this.streamReader.Dispose();
            // 若没有此键, 代表服务器错误产生
            if (JObject.Parse(rst).ContainsKey("success_trans"))
            {
                // 获取值-译文
                return (string)JObject.Parse(rst).First.ToObject(typeof(string));
            }
            return null;
        }

        /// <summary>
        /// 请求WolfamAlpha, 获取答案
        /// </summary>
        /// <param name="englishTQuestion">已经翻译成英文的文本</param>
        /// <returns></returns>
        private string GetWolfamAlphaAnwser(string englishTQuestion)
        {
            /* 
            下行Url访问自己的服务器获取WolfamAlpha返回的答案，你必须确保服务器已开启。需要处理返回的结果，并在此处直接接收，
            相关服务器脚本已经给出，参见根目录README.md
            当然，如果可以，你可以直接在此处处理API返回的结果，而不是用本人给出的脚本，
            需要请查看https://products.wolframalpha.com/short-answers-api/documentation/
            */
            string ownerUri = "http://localhost/as/index.php";
            // string ownerUri = "http://www.coollsx.com/api-station/transfer/.../short-a";
            string qUri = ownerUri + "?query=" + englishTQuestion;
            this.request = (HttpWebRequest)WebRequest.Create(qUri);
            this.request.Method = "GET";
            this.request.AllowAutoRedirect = true;
            this.request.ContentType = null;
            this.request.Timeout = 15000;
            this.response = (HttpWebResponse)request.GetResponse();
            this.streamReader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string englishAnwser = streamReader.ReadToEnd();
            this.response.Close();
            this.response.Dispose();
            this.streamReader.Close();
            this.streamReader.Dispose();
            return englishAnwser;
        }
    }
}
