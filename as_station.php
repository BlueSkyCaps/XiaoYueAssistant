<?php

if (!($_SERVER['REQUEST_METHOD'] == 'GET')){
    return;
}

$get_query = $_GET["query"];
$appId = "你的WolframAlpha Short-Answer appid";
$uri = "http://api.wolframalpha.com/v1/result?appid=" . $appId;
$qUri = $uri . "&i=" . urlencode($get_query) . "&units=metric&timeout=15";

/* 发起http get请求 */
$response_answer =  request_by_curl($qUri);
// 直接返回调用方，给调用方处理
echo $response_answer;

function request_by_curl($remote_server) {
    $curl_handle = curl_init();
    curl_setopt($curl_handle, CURLOPT_URL, $remote_server);
    curl_setopt($curl_handle, CURLOPT_HEADER, false);
    curl_setopt($curl_handle, CURLOPT_HTTPGET, true);
    //curl_setopt($curl_handle, CURLOPT_USERAGENT, 'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36');
    curl_setopt($curl_handle, CURLOPT_TIMEOUT, 15);
    curl_setopt($curl_handle, CURLOPT_RETURNTRANSFER, true);
    $answer = curl_exec($curl_handle);
    curl_close($curl_handle);

    return $answer;
}
