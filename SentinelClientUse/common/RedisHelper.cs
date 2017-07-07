using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentinelClientUse.common
{
    public class RedisHelper
    {
        private static string hostPrefix = @":";
        private static string separatedPrefix = @",";
        public static string ReadReidsConfigs { get; set; }
        public static string WriteRedisConfigs { get; set; }
        public static void SubscriberSentinelEvent(string sentinelConfigs)
        {
            Check.SentinelConfigIsTrue(sentinelConfigs);

            ConfigurationOptions sentinelConfig = ConfigSentinel(sentinelConfigs);
            sentinelConfig.TieBreaker = "";
            sentinelConfig.CommandMap = CommandMap.Sentinel;
            sentinelConfig.DefaultVersion = new Version(3, 0);
            ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(sentinelConfig);
            GetRedisHostBySentinel(sentinelConfig, conn);
            string t1 = ReadReidsConfigs;
            string t2 = WriteRedisConfigs;
            ISubscriber sub = conn.GetSubscriber();
            sub.Subscribe("+sdown", (channel, message) =>
            {
                UpdateConfigs(message);
                Console.WriteLine("服务端地址改变：" + message + ",channel:" + channel);
            });
            sub.Subscribe("-sdown", (channel, message) =>
            {
                UpdateConfigs(message);
                Console.WriteLine("服务端地址改变：" + message + ",channel:" + channel);
            });
        }

        private static void UpdateConfigs(RedisValue message)
        {
            string[] strs = message.ToString().Split(' ');
            if (strs[0] == "slave")
                WriteRedisConfigs.Replace(separatedPrefix + strs[1] + hostPrefix + strs[2], "");
            if (strs[0] == "master")
                WriteRedisConfigs.Replace(separatedPrefix + strs[2] + hostPrefix + strs[3], "");
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
            StringBuilder masterAdress = new StringBuilder();
            StringBuilder slavesAdress = new StringBuilder();
            int count = _svrList.Count();
            foreach (var svr in _svrList)
            {
                KeyValuePair<string, string>[][] masterPair = svr.SentinelMasters();
                masterAdress.Append(masterPair[0][1].Value + hostPrefix + masterPair[0][2].Value);
                masterAdress.Append(",");

                var slavesPair = svr.SentinelSlaves((masterPair[0])[0].Value);
                int sCount = slavesPair.Count();
                foreach (var slave in slavesPair)
                {
                    slavesAdress.Append((slave)[0].Value);
                    slavesAdress.Append(",");
                }
            }
            ReadReidsConfigs = masterAdress.ToString().Substring(0,masterAdress.Length-1);
            WriteRedisConfigs = slavesAdress.ToString().Substring(0,slavesAdress.Length-1);
        }
    }
}
