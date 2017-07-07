using CSRedis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CsredisUse
{
    class Program
    {
        static void Main(string[] args)
        {

            //using (var sentinel = new RedisSentinelClient("192.168.10.10", 6000))
            //{
            //    bool flag = true;
            //    //sentinel.SubscriptionReceived += (s, e)=>{
            //    while (flag)
            //    {
            //        Thread.Sleep(1000);
            //        var master = sentinel.Slaves("0master");//这个就是在Sentinel上面为Master主机起的名字，要一致 
            //        foreach (var item in master)
            //        {
            //            Console.WriteLine(item.MasterHost + ":" + item.MasterPort);//得到Master机器信息 
            //            Console.WriteLine(item.Ip + ":" + item.Port);//得到Slave机器信息 
            //            Console.WriteLine();
            //            flag = false;
            //        }
            //    }
            //    //};
            //}
            //Console.Read();
            #region 基本使用
            //using (var redis = new RedisClient("192.168.10.11", 7000))
            //{
            //    string ping = redis.Ping();
            //    string echo = redis.Echo("hello world");//这里的echo表示发送给服务器什么，服务器就返回什么，也就是echo命令
            //    DateTime time = redis.Time();//这里表示向服务器发送time命令
            //    redis.Set("key2", "value2");
            //}
            #endregion
            #region 异步方法
            //using (var redis = new RedisClient("192.168.10.11", 7002))
            //{
            //    for (int i = 0; i < 5000; i++)
            //    {
            //        redis.IncrAsync("test1");//这个命令也是redis命令，向键为test1的value递增1
            //    }

            //    // 回调函数 ContinueWith: Ping是异步执行的，当结果准备好时，响应会被打印到屏幕上。
            //    redis.TimeAsync().ContinueWith(t => Console.WriteLine(t.Result));

            //    // 阻塞调用
            //    string result = redis.GetAsync("test1").Result;
            //}
            #endregion
            #region 以管道方式执行命令
            //using (var redis = new RedisClient("192.168.10.11", 7001))
            //{
            //    redis.StartPipe();
            //    var empty1 = redis.Echo("hello"); // 返回默认值
            //    var empty2 = redis.Time(); // 返回默认时间
            //    object[] result = redis.EndPipe(); // 所有命令会马上发往服务端
            //    var item1 = (string)result[0]; // 将结果对象转换为适当类型
            //    var item2 = (DateTime)result[1];

            //    // 启动事务管道
            //    redis.StartPipeTransaction();
            //    redis.Set("key", "value");
            //    redis.Set("key2", "value2");
            //    object[] result2 = redis.EndPipe();

            //    // 抛弃管道事务
            //    redis.StartPipeTransaction();
            //    redis.Set("key", 123);
            //    redis.Set("key2", "abc");
            //    redis.Discard();
            //}
            #endregion
            #region 散列方法
            //            using (var redis = new RedisClient("192.168.10.11", 7002))
            //            {
            //                redis.HMSet("myhash", new
            //                {
            //                    Field1 = "string",
            //                    Field2 = true,
            //                    Field3 = DateTime.Now,
            //                });

            //                MyPOCO hash = redis.HGetAll<MyPOCO>("my-hash-key");

            //                redis.HMSet("mydict", new Dictionary<string, string>
            //{
            //  { "F1", "string" },
            //  { "F2", "true" },
            //  { "F3", DateTime.Now.ToString() },
            //});
            //                redis.HMSet("myhash", new[] { "F1", "string", "F2", "true", "F3", DateTime.Now.ToString() });
            //            }
            #endregion
            #region 事务
            //using (var redis = new RedisClient("192.168.10.11", 7001))
            //{
            //    redis.TransactionQueued += (s, e) =>
            //{
            //    Console.WriteLine("Transaction queued: {0}({1}) = {2}", e.Command, String.Join(", ", e.Arguments), e.Status);
            //};
            //    redis.Multi();
            //    var empty1 = redis.Set("test1", "hello"); // returns default(String)
            //    var empty2 = redis.Set("test2", "world"); // returns default(String)
            //    var empty3 = redis.Time(); // returns default(DateTime)
            //    object[] result = redis.Exec();
            //    var item1 = (string)result[0];
            //    var item2 = (string)result[1];
            //    var item3 = (DateTime)result[2];
            //}
            #endregion
            #region 订阅模型
            //using (var redis = new RedisClient("192.168.10.11", 7002))
            //{
            //    redis.SubscriptionChanged += (s, e) =>
            //{
            //    Console.WriteLine("There are now {0} open channels", e.Response.Count);
            //};
            //    redis.SubscriptionReceived += (s, e) =>
            //    {
            //        Console.WriteLine("Message received: {0}", e.Message.Body);
            //    };
            //    redis.PSubscribe("*");
            //}
            #endregion
            #region 执行redis命令-支持任何命令，并且不会过时
            //using (var redis = new RedisClient("192.168.10.11", 7002))
            //{
            //    object resp = redis.Call("ANYTHING", "arg1", "arg2", "arg3");
            //}
            #endregion
            #region 流媒体的写入和读取
            //using (var redis = new RedisClient("192.168.10.11", 7002))
            //{
            //    redis.Set("test", new string('x', 1048576)); // 1MB string
            //    using (var ms = new MemoryStream())
            //    {
            //        redis.StreamTo(ms, 64, r => r.Get("test")); // read in small 64 byte blocks
            //        byte[] bytes = ms.ToArray(); // optional: get the bytes if needed
            //    }
            //}
            #endregion
            #region 哨兵连接管理支持
            //using (var sentinel = new RedisSentinelManager("192.168.10.10:6000", "192.168.10.10:6001", "192.168.10.10:6002"))
            //{
            //    //sentinel.Add("6000"); // add host using default port 
            //    //sentinel.Add("6001", 36379); // add host using specific port
            //    //sentinel.Connected += (s, e) => sentinel.Call(x => x.Auth("531488869")); // this will be called each time a master connects
            //    sentinel.Connect("0master"); // open connection
            //    var test2 = sentinel.Call(x => x.Time()); // use the Call() lambda to access the current master connection
            //}
            #endregion
        }

        public class MyPOCO
        {
            public string Field1 { get; set; }
            public bool Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }
    }
}
