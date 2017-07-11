using SentinelClientUseMange.common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelClientUseMange
{
    class Program
    {
        static void Main(string[] args)
        {
            //配置
            string sentinelConfig = "192.168.10.10:6000:0master,192.168.10.10:6001:1master,192.168.10.10:6002:2master";
            string redisConfig = "192.168.10.10:7004,192.168.10.10:7003,192.168.10.11:7001";
            //RedisHelper.Init(redisConfig);
            RedisHelper.SubscriberSentinelEvent(sentinelConfig);
            //var redis=RedisHelper.GetRdisInstance();
            //使用
            //IDatabase db = redis.GetDatabase();
            //字符串操作
            //db.StringSet("cxb", "谁啊你");
            //string Value = db.StringGet("cxb");
            //Console.WriteLine(Value);

            Console.Read();
        }
    }
}
