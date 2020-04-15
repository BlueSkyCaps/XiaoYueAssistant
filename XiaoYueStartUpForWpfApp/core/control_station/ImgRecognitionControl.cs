using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.control_station
{
    /// <summary>
    /// 图像识别交互控制类
    /// </summary>
    class ImgRecognitionControl : GeneralControlStation
    {
        public ImgRecognitionControl(string type, string imgPath)
        {
            Console.WriteLine(imgPath);
            Image img = Image.FromFile(imgPath);
            if (img.Width > 4096 || img.Height > 4096 || img.Width < 15 || img.Width < 15)
            {
                System.Windows.Forms.MessageBox.Show("图片像素过大？低于30px？？");
                img.Dispose();
                return;
            }
            img.Dispose();
            if (type == "imageR")
            {
                string rst = ImageRecongnitionUsual(imgPath);
                if (rst == "EP_FILEERROR" || rst == "EP_HTTPCONNECT")
                {
                    Console.WriteLine("糟糕！XiaoYue遇到了问题...");
                    return;
                }
                else
                    CollatingJsonData(rst, imgPath, type);
            }
            else if (type == "textimgOCR")
            {
                string rst = TextImageRecongnitionUsual(imgPath);
                if (rst == "EP_FILEERROR" || rst == "EP_HTTPCONNECT")
                {
                    Console.WriteLine("糟糕！XiaoYue遇到了问题...");
                    return;
                }
                else
                    CollatingJsonData(rst, imgPath, type);
            }
        }

        /// <summary>
        /// 图像识别调用接口。识别图像中的物体并返回数据
        /// </summary>
        /// <param name="getImgPath">传递的图片本地路径</param>
        /// <returns>Json格式的识别数据, 或异常字符串</returns>
        /// 注意: 连接的uri是自身服务器, 作为中转站, 由自身服务器向API传递数据并返回
        private string ImageRecongnitionUsual(string getImgPath)
        {
            string ownUri = "http://www.coollsx.com/api-station/transfer/bd/b73af658bcc5fb4a963c8a1ee0d05e2a/img-rg/object";
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ownUri + "/index.php");
            request.Method = "POST";
            request.AllowAutoRedirect = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
            string GetImgBase64(string imgPath)
            {
                try
                {
                    FileStream imgFilestream = new FileStream(imgPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    // 判断图片的字节大小。 像素大小已在之前判断, 因为在此处使用Image对象会影响当前流
                    if (imgFilestream.Length > (long)4194304)
                    {
                        return "EP_OVER";
                    }
                    byte[] imgByteArr = new byte[imgFilestream.Length];
                    imgFilestream.Read(imgByteArr, 0, (int)imgFilestream.Length);
                    string imgDataBase64 = Convert.ToBase64String(imgByteArr);
                    imgFilestream.Close();
                    imgFilestream.Dispose();
                    return imgDataBase64;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.TargetSite);
                    return "EP_FILEIO";
                }
            }
            string imgBase64 = GetImgBase64(getImgPath);
            if (imgBase64 == "EP_FILEIO" || imgBase64 == "EP_OVER")
            {
                return "EP_FILEERROR";
            }
            string str = "image=" + WebUtility.UrlEncode(imgBase64) + "&baike_num=1";
            byte[] paramBuffer = encoding.GetBytes(str);
            request.ContentLength = paramBuffer.Length;
            request.GetRequestStream().Write(paramBuffer, 0, paramBuffer.Length);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string rst = reader.ReadToEnd();
                Console.WriteLine(rst);
                response.Close();
                response.Dispose();
                reader.Close();
                reader.Dispose();
                return rst;
            }
            catch (Exception)
            {
                return "EP_HTTPCONNECT";
            }
        }

        /// <summary>
        /// 文字识别调用接口。识别图像中的文字并返回数据
        /// </summary>
        /// <param name="getImgPath">传递的图片本地路径(TODO 或图片http-url 但目前不需要)</param>
        /// <returns>Json格式的识别数据, 或异常字符串</returns>
        /// 注意: 连接的uri是自身服务器, 作为中转站, 由自身服务器向API传递数据并返回
        private string TextImageRecongnitionUsual(string getImgPath)
        {
            string ownerUri = "http://www.coollsx.com/api-station/transfer/bd/b73af658bcc5fb4a963c8a1ee0d05e2a/text-rg/general";
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ownerUri + "/index.php");
            request.Method = "POST";
            request.AllowAutoRedirect = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
            string GetImgBase64(string imgPath)
            {
                try
                {
                    FileStream imgFilestream = new FileStream(imgPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    if (imgFilestream.Length > (long)4194304)
                    {
                        return "EP_OVER";
                    }
                    byte[] imgByteArr = new byte[imgFilestream.Length];
                    imgFilestream.Read(imgByteArr, 0, (int)imgFilestream.Length);
                    string imgDataBase64 = Convert.ToBase64String(imgByteArr);
                    imgFilestream.Close();
                    imgFilestream.Dispose();
                    return imgDataBase64;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.TargetSite);
                    return "EP_FILEIO";
                }
            }
            string imgBase64 = GetImgBase64(getImgPath);
            if (imgBase64 == "EP_FILEIO" || imgBase64 == "EP_OVER")
            {
                return "EP_FILEERROR";
            }
            string str = "image=" + WebUtility.UrlEncode(imgBase64);
            byte[] paramBuffer = encoding.GetBytes(str);
            request.ContentLength = paramBuffer.Length;
            request.GetRequestStream().Write(paramBuffer, 0, paramBuffer.Length);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string rst = reader.ReadToEnd();
                response.Close();
                response.Dispose();
                reader.Close();
                reader.Dispose();
                return rst;
            }
            catch (Exception)
            {
                return "EP_HTTPCONNECT";
            }
        }

        /// <summary>
        /// 整理数据以显示
        /// </summary>
        /// <param name="rst">处理的最终识别结果</param>
        private void CollatingJsonData(string rst, string imgPath, string type)
        {
            Console.WriteLine(rst);
            JObject jObject = JObject.Parse(rst);
            if (jObject.ContainsKey("error_code"))
            {
                System.Windows.Forms.MessageBox.Show("眼睛疼！前方遇到了问题...");
                return;
            }
            // 图像物体识别结果
            if (type == "imageR")
            {
                Console.WriteLine(rst);
                string keyword = jObject.SelectToken("result[0].keyword").ToString();
                string root = jObject.SelectToken("result[0].root").ToString();
                if (!jObject.SelectToken("result[0].baike_info").HasValues)
                {
                    // 如果没有相关的百科词条
                    new core.page.secondary.recognition.RecognitionResultWin(keyword+"\r\n"+root+"\r\n", imgPath);
                    return;
                }
                string description = jObject.SelectToken("result[0].baike_info.description").ToString();
                string conStr = keyword + "\r\n" + root + "\r\n" + description + "\r\n";
                // 显示结果
                new core.page.secondary.recognition.RecognitionResultWin(conStr, imgPath);
                Console.WriteLine(keyword);
                Console.WriteLine(root);
                Console.WriteLine(description);
            }
            // ocr识别结果
            else if (type == "textimgOCR")
            {
                int words_result_num = (int)jObject.SelectToken("words_result_num").ToObject(typeof(int));
                if (words_result_num == 0)
                {
                    new core.page.secondary.recognition.RecognitionResultWin("看不出来有文字..", imgPath);
                    return;
                }
                var words = from ws in jObject["words_result"] select ws["words"];
                string conStr = "";
                foreach (var item in words)
                {
                    conStr += item + "\r\n";
                }
                // 显示结果
                new core.page.secondary.recognition.RecognitionResultWin(conStr, imgPath);
            }
            return;
        }
    }
}
