namespace XiaoYueStartUpForWpfApp.parameters.settings.server.connect
{
    struct ConnectingArgs
    {
        public struct RedisArgs
        {
            public string RedisHost => "192.168.194.129:6379";
            public string RedisPassword => "azxcv";
        }

        public RedisArgs RedisArgsObj => new RedisArgs();
    }
}
