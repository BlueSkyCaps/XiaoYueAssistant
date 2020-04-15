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
    /// 副界面输入框交互控制器
    /// 处理用户的输入文本, 响应答案
    /// 此处输入框无法处理任务命令, 它只提供问答功能。
    /// </summary>
    /// 此控制器控制副界面的文字问答交互 且决定不忽略标点符号, walframAlpha能够进行数学计算
    /// 因此最终结果与语音交互的结果可能不同:
    /// 诸如多余标点会对walframAlpha Short Anwser有影响
    class SecondaryInterSearchControl : GeneralControlStation
    {
        /// <summary>
        /// 要询问的问题
        /// </summary>
        private string queryText;
        /// <summary>
        /// 翻译成英文或中文的文本(WolframAlpha需要英文查询问题, 显示答案需要转为中文)
        /// </summary>
        private string transText;
        /// <summary>
        /// 最终有效的答案
        /// </summary>
        private string answerText;
        /// <summary>
        /// 副界面输入框源
        /// </summary>
        private System.Windows.Controls.TextBox inputBoxObj;
        /// <summary>
        /// 响应的界面
        /// </summary>
        private static page.other.SpeechTextDisplay speechTextDisplay = new page.other.SpeechTextDisplay("", "");
        /// <summary>
        /// 这是WolframAlpha无匹配的回应
        /// </summary>
        private readonly string _waNoneAnswer = "No short answer available";
        /// <summary>
        /// 这是WplframAlpha无法理解问题的回应
        /// </summary>
        private readonly string _waNotUnderstand = "Wolfram|Alpha did not understand your input";
        /// <summary>
        /// 捕获到的网络异常, 需要提示用户的字符串
        /// </summary>
        private readonly string _connectFailed = "Error:" + "\n" + "网络连接异常..";
        private StreamReader streamReader;
        private HttpWebRequest request;
        private HttpWebResponse response;
        /// <summary>
        /// 副界面的输入框允许用户输入文本。通过数据处理顺序以得到有效的答案来响应
        /// 若询问的文本内容无法得到解答, 回应一段语音
        /// </summary>
        /// <param name="text">输入的查询文本</param>
        /*
         * 数据模型调用顺序:
         * 函数迭代
         * 首先查询用户自建的本地数据(通过魔法训练中自定义的回答) ? None -> 
         * 远程数据库 ? None ->
         * WolframAlpha ? None -> ...
         */
        public SecondaryInterSearchControl(string qText, System.Windows.Controls.TextBox inputBoxObj)
        {
            this.inputBoxObj = inputBoxObj;
            this.queryText = qText;
            Console.WriteLine(this.queryText);
            string rst = QueryCustomAnswerXY(this.queryText);

            // 翻译至英文过程中失败
            if (rst == "TRANS_TO_EN_FAILED")
            {
                if (SecondaryInterSearchControl.speechTextDisplay.IsVisible)
                {
                    SecondaryInterSearchControl.speechTextDisplay.your_msg_display.Text = this.queryText;
                    SecondaryInterSearchControl.speechTextDisplay.xy_msg_display.Text = "哎呀，出错了..";
                }
                else
                {
                    SecondaryInterSearchControl.speechTextDisplay = new page.other.SpeechTextDisplay(this.queryText, "哎呀，出错了..");
                    SecondaryInterSearchControl.speechTextDisplay.Show();
                }
                return;
            }

            /**
             * 显示响应的界面 注: 如果翻译至中文时发生错误, 代表之前的英文操作执行正常 显示英文的答案
             * 否则 中英文将一并显示
             * 如果异常 连接问题, 显示的是_connectFailed
             */
            Console.WriteLine("最终答案：" + this.answerText);
            if (SecondaryInterSearchControl.speechTextDisplay.IsVisible)
            {
                SecondaryInterSearchControl.speechTextDisplay.your_msg_display.Text = this.queryText;
                SecondaryInterSearchControl.speechTextDisplay.xy_msg_display.Text = this.answerText;
            }
            else {
                SecondaryInterSearchControl.speechTextDisplay = new page.other.SpeechTextDisplay(this.queryText, this.answerText);
                SecondaryInterSearchControl.speechTextDisplay.Show();
            }
        }

        /// <summary>
        /// 查询本地自定义问答库
        /// </summary>
        /// <param name="qText"></param>
        /// <returns></returns>
        private string QueryCustomAnswerXY(string qText)
        {
            string data = File.ReadAllText("parameters/settings/server/custom-data/user-custom-data.json", Encoding.UTF8);
            JObject jObject = JObject.Parse(data);
            if (jObject["qas"][qText] != null)
            {
                this.answerText = (string)jObject["qas"][qText];
                return this.answerText;
            }
            return QueryRedisDB(qText);
        }

        /// <summary>
        /// 查询远程Redis数据库
        /// </summary>
        /// <returns></returns>
        private string QueryRedisDB(string qText) {
            try
            {
                RedisDbControl redisDbControl = new RedisDbControl(qText);
                object value = redisDbControl.GetAnswer();
                if (!(value is false))
                {
                    this.answerText = value.ToString(); //不允许强制转换RdisValue类型
                    return this.answerText;
                }
            }
            catch (Exception)
            {
                return WolframAlphaResponse(qText);
            }
            return WolframAlphaResponse(qText);
        }

        /// <summary>
        /// 传递参数给服务器, 服务器调用WolframAlpha并返回Answer
        /// </summary>
        /// <param name="qText"></param>
        /// <returns></returns>
        private string WolframAlphaResponse(string qText)
        {
            try
            {
                /* 先调用自身服务器 翻译成英文 */
                string transRst = TranslationRing(qText, "en");

                // 翻译至英文过程中失败
                if (transRst == "TRANS_TO_EN_FAILED")
                {
                    return transRst;
                }

                /* 请求服务器 传递英文 获取答案 */
                string waAnwser = GetWolfamAlphaAnwser(transRst);
                if (waAnwser == _waNoneAnswer || waAnwser == _waNotUnderstand)
                {
                    /**
                     * 若WolframAlpha没有匹配的答案, 可以将原问题(非英文)继续匹配其余的数据 否则, 结束
                     * TODO
                     */
                    this.answerText = "抱歉，我暂时不明白..";
                    // todo 此处可以响应一段语音
                    return this.answerText;
                }
                else
                {
                    Console.WriteLine(waAnwser);
                    this.inputBoxObj.Text = "";
                    // 还需将WolframAlpha获取的答案转为中文显示
                    transRst = TranslationRing(waAnwser, "zh");
                    // 如果在翻译至中文过程中失败, 显示英文答案 否则, 英文+中文一并显示
                    if (transRst == "TRANS_TO_ZH_FAILED")
                    {
                        this.answerText = waAnwser;
                        return this.answerText;
                    }
                    this.answerText = transRst + waAnwser;
                    return this.answerText;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                // 发生连接异常, 最终答案设为此, 以便提示
                this.answerText = this._connectFailed;
                return this.answerText;
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
            string ownerUri = "http://www.coollsx.com/api-station/transfer/bd/b73af658bcc5fb4a963c8a1ee0d05e2a/text-tr/general";
            string qUri = ownerUri + "/index.php";
            this.request = (HttpWebRequest)WebRequest.Create(qUri);
            this.request.Method = "POST";
            this.request.KeepAlive = true;
            this.request.AllowAutoRedirect = false;
            this.request.Timeout = 15000;
            this.request.ContentType = "application/x-www-form-urlencoded";
            string fields = "code=" + "c5e8962fb24a491670e1e79a8702439b" + "&original=" + WebUtility.UrlEncode(originalText) + "&to=" + to;
            byte[] fields_bytes = encoding.GetBytes(fields);
            this.request.GetRequestStream().Write(fields_bytes, 0, fields_bytes.Length);
            this.response = (HttpWebResponse)request.GetResponse();
            this.streamReader = new System.IO.StreamReader(this.response.GetResponseStream(), Encoding.UTF8);
            string rst = streamReader.ReadToEnd();
            // 若没有此键, 代表错误产生
            if (JObject.Parse(rst).ContainsKey("success_trans"))
            {
                // 获取值-译文
                this.transText = (string)JObject.Parse(rst).First.ToObject(typeof(string));
            }
            else
            {
                if (to == "en")
                {
                    Console.WriteLine("翻译至英文失败");
                    this.transText = "TRANS_TO_EN_FAILED";
                }
                else
                {
                    Console.WriteLine("翻译至中文失败");
                    this.transText = "TRANS_TO_ZH_FAILED";
                }
            }
            this.response.Close();
            this.response.Dispose();
            this.streamReader.Close();
            this.streamReader.Dispose();

            return this.transText;
        }

        /// <summary>
        /// 请求服务器, 获取答案
        /// </summary>
        /// <param name="englishText">已经翻译成英文的文本</param>
        /// <returns></returns>
        private string GetWolfamAlphaAnwser(string englishText)
        {
            string ownerUri = "http://www.coollsx.com/api-station/transfer/wa/b73af658bcc5fb4a963c8a1ee0d05e2a/short-a";
            string qUri = ownerUri + "?query=" + englishText;
            this.request = (HttpWebRequest)WebRequest.Create(qUri);
            this.request.Method = "GET";
            this.request.AllowAutoRedirect = true;
            this.request.ContentType = null;
            this.request.Timeout = 15000;
            this.response = (HttpWebResponse)request.GetResponse();
            this.streamReader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string anwser = streamReader.ReadToEnd();
            this.response.Close();
            this.response.Dispose();
            this.streamReader.Close();
            this.streamReader.Dispose();

            return anwser;
        }
    }
}
