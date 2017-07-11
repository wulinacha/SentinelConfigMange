using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SentinelClientUseMange.common
{
    public class Check
    {
        public static void SentinelConfigIsTrue(string configs)
        {
            string[] splitConfigs = CheckIsNullAndSplit(configs);

            for (int i = 0; i < splitConfigs.Count(); i++)
            {
                if (splitConfigs[i].Split(':')[0] == "" || splitConfigs[i].Split(':')[1] == "" || splitConfigs[i].Split(':')[2] == "")
                    throw new Exception("配置元素不能为空");
                CheckRule(splitConfigs, i);
            }
        }

        public static void RedisConfigIsTrue(string configs)
        {
            string[] splitConfigs = CheckIsNullAndSplit(configs);

            for (int i = 0; i < splitConfigs.Count()-1; i++)
            {
                if (splitConfigs[i].Split(':')[0] == "" || splitConfigs[i].Split(':')[1] == "")
                    throw new Exception("配置元素不能为空");
                CheckRule(splitConfigs, i);
            }
        }
        private static string[] CheckIsNullAndSplit(string configs)
        {
            if (configs == null)
                throw new Exception("配置不能为空");

            string[] splitConfigs = configs.Split(',');
            return splitConfigs;
        }
        private static void CheckRule(string[] splitConfigs, int i)
        {
            if (splitConfigs[i].Split(':').Count() < 2)
                throw new Exception("配置元素不能为空");
            if (!Regex.IsMatch(splitConfigs[i].Split(':')[1], @"^[+-]?\d*[.]?\d*$"))
                throw new Exception("配置不符合要求");
            if (!Regex.IsMatch(splitConfigs[i].Split(':')[0], @"[0-9]*[.][0-9]*[.][0-9]*"))
                throw new Exception("配置不符合要求");
        }
    }
}
