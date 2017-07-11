using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentinelClientUseMange.common
{
    public class RedisHelper
    {
        private static string hostPrefix = @":";
        private static string separatedPrefix = @",";
        private static List<string> readReidsConfigs = new List<string>();// private static ConcurrentDictionary<string,string> readReidsConfigs = new ConcurrentDictionary<string, string>();
        private static List<string> writeRedisConfigs = new List<string>();
        public static string GetReadReidsConfigs() {
            if (readReidsConfigs.Count <= 0)
                return "尚未初始化";
            return string.Join(",", readReidsConfigs.ToArray());
        }
        public static string GetWriteRedisConfigs() {
            if (writeRedisConfigs.Count <= 0)
                return "尚未初始化";
            return string.Join(",", writeRedisConfigs.ToArray());
        }
        public static void SubscriberSentinelEvent(string sentinelConfigs)
        {
            Check.SentinelConfigIsTrue(sentinelConfigs);

            ConfigurationOptions sentinelConfig = ConfigSentinel(sentinelConfigs);
            sentinelConfig.TieBreaker = "";
            sentinelConfig.CommandMap = CommandMap.Sentinel;
            sentinelConfig.DefaultVersion = new Version(3, 0);
            ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(sentinelConfig);
            GetRedisHostBySentinel(sentinelConfig, conn);

            string t1 = GetReadReidsConfigs();
            string t2 = GetWriteRedisConfigs();

            ISubscriber sub = conn.GetSubscriber();
            sub.Subscribe("+sdown", (channel, message) =>
            {
                DeleteConfigs(message);
                Console.WriteLine("服务端地址改变：" + message + ",channel:" + channel);
            });
            sub.Subscribe("-sdown", (channel, message) =>
            {
                AddConfigs(message);
                Console.WriteLine("服务端地址改变：" + message + ",channel:" + channel);
            });
        }

        private static void DeleteConfigs(RedisValue message)
        {
            string[] strs = message.ToString().Split(' ');
            if (strs[0] == "slave")
                readReidsConfigs.Remove(strs[1]);
            if (strs[0] == "master")
                writeRedisConfigs.Remove(separatedPrefix + strs[2] + hostPrefix + strs[3]);
        }

        private static void AddConfigs(RedisValue message)
        {
            string[] strs = message.ToString().Split(' ');
            if (strs[0] == "slave")
                readReidsConfigs.Add(strs[1]);
            if (strs[0] == "master")
                writeRedisConfigs.Add(separatedPrefix+strs[2] + hostPrefix + strs[3]);
        }

        private static ConfigurationOptions ConfigSentinel(string sentinelConfigs)
        {
            string[] splitConfigs = sentinelConfigs.Split(',');

            ConfigurationOptions sentinelConfig = new ConfigurationOptions();
            for (int i = 0; i < splitConfigs.Count(); i++)
            {
                sentinelConfig.ServiceName = splitConfigs[i].Split(':')[2];
                sentinelConfig.EndPoints.Add(splitConfigs[i].Split(':')[0], int.Parse(splitConfigs[i].Split(':')[1]));
            }

            return sentinelConfig;
        }

        private static void GetRedisHostBySentinel(ConfigurationOptions sentinelConfig, ConnectionMultiplexer conn)
        {
            List<IServer> _svrList = new List<IServer>();
            foreach (var item in sentinelConfig.EndPoints)
            {
                _svrList.Add(conn.GetServer(item));
            }
            int count = _svrList.Count();
            foreach (var svr in _svrList)
            {
                KeyValuePair<string, string>[][] masterPair = svr.SentinelMasters();
                readReidsConfigs.Add(masterPair[0][1].Value + hostPrefix + masterPair[0][2].Value);

                var slavesPair = svr.SentinelSlaves((masterPair[0])[0].Value);
                int sCount = slavesPair.Count();
                foreach (var slave in slavesPair)
                {
                    writeRedisConfigs.Add((slave)[0].Value);
                }
            }
        }


    }
}
