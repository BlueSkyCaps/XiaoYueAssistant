<?php

define("CURL_TIMEOUT", 10);
define("URL", "http://api.fanyi.baidu.com/api/trans/vip/translate");
define("APP_ID", "你的翻译api Appid"); //
define("SEC_KEY", "你的翻译api 密钥"); //
if (!($_SERVER['REQUEST_METHOD'] == 'POST')){
    return;
}

$get_request_array = $_POST;

/*
 * 原语言都是auto
 * post请求需要的参数: code 鉴权码/ original 原文文本/ to 目标语言
 * */
if (array_key_exists('code', $get_request_array)) {
    if (!(isset($get_request_array['code']) && $get_request_array['code'] == 'c5e8962fb24a491670e1e79a8702439b')){
        echo "WrongAuthenticationCode";
        return;
    }else{
        if (array_key_exists('original', $get_request_array)){
            if (trim($get_request_array['original']) !== ""){
                if (array_key_exists('to', $get_request_array) && trim($get_request_array['to']) !== ""){
                    rst_response($get_request_array['original'], 'auto', $get_request_array['to']);
                }else{
                    echo "TargetLanguageIsEmpty";
                }
            }else{
                echo "OriginalTextIsEmpty";
                return;
            }
        }else {
            echo "OriginalTextNotExist";
            return;
        }
    }
}else{
    echo "NoAuthenticationCode";
    return;
}


function rst_response($original, $from='auto', $to){
    $ret = translate($original, $from, $to);
    $trans_str_joint = '';
    // 若存在error_code, 返回错误码。array_key_exists()仅搜索第一维的键。多维数组里嵌套的键不会被搜索到
    if (array_key_exists("error_code", $ret)){
        echo "error_code is " . $ret['error_code'];
        return;
    }
    // 如果原文本有换行, api返回多个翻译结果(json, 按array处理), 遍历以连接
    foreach ($ret as $item => $value){
        if (is_array($value)){
            foreach ($value as $k => $v){
                if (is_array($v)){
                    foreach ($v as $i => $j){
                        if (is_array($j)){
                            continue;
                        }else{
                            if ($i == 'dst')
                                $trans_str_joint.=$j.PHP_EOL;
                        }
                    }
                }
            }
        }
    }
    // 返回译文
    print_r(json_encode(array('success_trans' => $trans_str_joint)));
    return;
}

//翻译入口
function translate($query, $from, $to)
{
    $args = array(
        'q' => $query,
        'appid' => APP_ID,
        'salt' => rand(10000,99999),
        'from' => $from,
        'to' => $to,

    );
    $args['sign'] = buildSign($query, APP_ID, $args['salt'], SEC_KEY);
    $ret = call(URL, $args);
    $ret = json_decode($ret, true);
    return $ret;
}

//加密
function buildSign($query, $appID, $salt, $secKey)
{/*{{{*/
    $str = $appID . $query . $salt . $secKey;
    $ret = md5($str);
    return $ret;
}/*}}}*/

//发起网络请求
function call($url, $args=null, $method="post", $testflag = 0, $timeout = CURL_TIMEOUT, $headers=array())
{/*{{{*/
    $ret = false;
    $i = 0;
    while($ret === false)
    {
        if($i > 1)
            break;
        if($i > 0)
        {
            sleep(1);
        }
        $ret = callOnce($url, $args, $method, false, $timeout, $headers);
        $i++;
    }
    return $ret;
}/*}}}*/

function callOnce($url, $args=null, $method="post", $withCookie = false, $timeout = CURL_TIMEOUT, $headers=array())
{/*{{{*/
    $ch = curl_init();
    if($method == "post")
    {
        $data = convert($args);
        curl_setopt($ch, CURLOPT_POSTFIELDS, $data);
        curl_setopt($ch, CURLOPT_POST, 1);
    }
    else
    {
        $data = convert($args);
        if($data)
        {
            if(stripos($url, "?") > 0)
            {
                $url .= "&$data";
            }
            else
            {
                $url .= "?$data";
            }
        }
    }
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_TIMEOUT, $timeout);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
    if(!empty($headers))
    {
        curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
    }
    if($withCookie)
    {
        curl_setopt($ch, CURLOPT_COOKIEJAR, $_COOKIE);
    }
    $r = curl_exec($ch);
    curl_close($ch);
    return $r;
}/*}}}*/

function convert(&$args)
{/*{{{*/
    $data = '';
    if (is_array($args))
    {
        foreach ($args as $key=>$val)
        {
            if (is_array($val))
            {
                foreach ($val as $k=>$v)
                {
                    $data .= $key.'['.$k.']='.rawurlencode($v).'&';
                }
            }
            else
            {
                $data .="$key=".rawurlencode($val)."&";
            }
        }
        return trim($data, "&");
    }
    return $args;
}/*}}}*/
