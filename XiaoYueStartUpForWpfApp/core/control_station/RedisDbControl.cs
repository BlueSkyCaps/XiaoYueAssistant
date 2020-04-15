using StackExchange.Redis;

namespace XiaoYueStartUpForWpfApp.core.control_station
{
    /// <summary>
    /// 操控Redis, 获取数据等。
    /// Key是Question, Value是Answer
    /// </summary>
    internal class RedisDbControl
    {
        private readonly string _question;
        private data.RedisDbData redisDb;
        /// <summary>
        /// 初始化RedisDbControl, 用question生成RedisDbData
        /// </summary>
        /// <param name="question"></param>
        public RedisDbControl(string question)
        {
            _question = question;
            redisDb = new data.RedisDbData(_question);
        }

        /// <summary>
        /// 获取Redis存储的答案, false若不存在
        /// </summary>
        /// <returns></returns>
        public object GetAnswer()
        {
            return redisDb.GetDbData(); 
        }

        // todo
        public bool AlterRdisKeyValue() { return false; }
        // todo
        public bool AddRedisKeyValue() { return false; }
    }
}