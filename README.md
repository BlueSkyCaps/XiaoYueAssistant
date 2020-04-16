# XiaoYueAssistant

WIndows上的一款智能语音助手，能够实现语音交互、定时任务、图像识别、自定义问答等。
此项目主要为一个问答助手，但也可以完成桌面任务。

1. Window 10下.Net Framework 4.6.1开发，Visual Studio 2017编码环境。
2. WPF框架构建界面。
2. ffmpeg完成音频编码任务。
3. 语音识别以及图像物体识别、OCR文字识别使用相关的百度RESTful API。
4. 优先使用WolframAlpha API完成智能交流问答。
5. 使用Redis作为后台服务端存储。
......

因为使用到wolframAlpha，因此需要英文去调用，后台将中文翻译成英语传入，又将获取到的答案转化为中文，最终界面会显示中+英文结果。

项目中已经存在可执行文件，可以直接运行测试。若发生问题，可能是相关接口未及时更新，不保证会持续更新此项目。
若要编译运行项目，需要修改相关代码，添加你自己的API 密钥，并执行以下操作：

1.
在XiaoYueStartUpForWpfApp\core\control_station\ImgRecognitionControl.cs中添加你自己的密钥，你需要使用百度图像识别的通用物体和场景识别高级版接口和通用文字识别接口。请创建相关应用得到appid,appkey,secretkey密钥。
最后，请访问https://ai.baidu.com/ai-doc/REFERENCE/Ck3dwjhhu 查看如何获取Access Token。

2.
开通百度语音识别接口，访问https://ai.baidu.com/tech/speech/asr
在XiaoYueStartUpForWpfApp\core\control_station\SpaceKeySpeechControl.cs中添加你的appid,appkey,secretkey

3.
启动Apache服务器，并在www根目录下创建fy、as目录，分别创建index.php。
XiaoYueStartUpForWpfApp\core\control_station\ResultThinkTheSpeechControl和SecondaryInterSearchControl类需要使用到，参见ResultThinkTheSpeechControl类中相关函数的注释说明。
/fy/index.php中添加项目根目录下文件fy_station中的代码，/as/index.php添加文件as_station中代码，并修改成自己的相关密钥。as_station中的WolframAlpha密钥，你需要申请，参见https://products.wolframalpha.com/short-answers-api/documentation/

4.
如果需要，XiaoYueStartUpForWpfApp\parameters\settings\server\connect\ConnectingArgs.cs定义了Redis服务器地址以及密码。

可以随时按下Esc键关闭项目运行进程。
