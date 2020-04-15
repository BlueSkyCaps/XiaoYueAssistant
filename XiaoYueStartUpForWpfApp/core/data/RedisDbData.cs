using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.data
{
    /// <summary>
    /// RedisDbData用于访问Redis, 由RedisDbControl操控
    /// </summary>
    class RedisDbData : IDataBaseData
    {
        private readonly string _key;
        private IDatabase db;
        // 连接的参数结构
        private parameters.settings.server.connect.ConnectingArgs connectingArgs =
            new parameters.settings.server.connect.ConnectingArgs();

        public RedisDbData(string key)
        {
            _key = key;
            ConfigurationOptions options = new ConfigurationOptions
            {
                ConnectTimeout = 5000,
                Password = connectingArgs.RedisArgsObj.RedisPassword,
                AllowAdmin = true
            };
            options.EndPoints.Add(connectingArgs.RedisArgsObj.RedisHost);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
            db = redis.GetDatabase();
        }

        public object GetDbData()
        {
            if (!db.KeyExists(_key))
                return false;
            return db.StringGet(_key);
        }

        //todo
        public bool AddDbData()
        {
            throw new NotImplementedException();
        }

        //todo
        public bool AlterDbData()
        {
            throw new NotImplementedException();
        }
    }
}
