using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace StackExchangeUse
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("192.168.10.15:7003,192.168.10.11:7002");
            //命令操作
            //IServer server = redis.GetServer("192.168.10.10", 7003);
            //string str=server.Echo("你好吗");

            IDatabase db = redis.GetDatabase();
            //字符串操作
            db.StringSet("cxb", "谁啊你");
            string Value = db.StringGet("cxb");
            Console.WriteLine(Value);

            //二进制操作
            string binaryStr = "二进制字符串=ni+你说呢 我不知道";
            byte[] toByte = NetworkInput(binaryStr);
            db.StringSet("二进制", toByte);
            byte[] resultToByte = db.StringGet("二进制");
            string result = NetworkOutPut(resultToByte);
            Console.WriteLine(result);

            //队列操作
            ISubscriber sub = redis.GetSubscriber();
            sub.Subscribe("sdown", (channel, message) =>
            {
                Console.WriteLine("客户I,消息:" + message);
            });
            ThreadPool.QueueUserWorkItem(QueueSub, sub);

            long flag = sub.Publish("bosco", "我订阅bosco频道");
            Console.Read();
        }

        private static void QueueSub(object state)
        {
            ISubscriber sub = (ISubscriber)state;
            sub.Subscribe("bosco", (channel, message) => {
                Console.WriteLine("客户II,消息:" + message);
            });
        }

        #region 网络传输
        private static string NetworkOutPut(byte[] toByte)
        {
            string returnByte = Encoding.UTF8.GetString(toByte);

            string returnParamt = DeParamFormat(returnByte);
            return Decryption(returnParamt);
        }

        private static byte[] NetworkInput(string binaryStr)
        {
            string strCv = Encryption(binaryStr);
            string paramt = EnParamFormat(strCv);
            return Encoding.UTF8.GetBytes(paramt);
        }

        private static string DeParamFormat(string resultCv)
        {
            resultCv = resultCv.Replace("a=", "");
            return HttpUtility.UrlDecode(resultCv);
        }

        private static string EnParamFormat(string strCv)
        {
            string urlCode = HttpUtility.UrlEncode(strCv);
            string paramt = $"a={urlCode}";
            return paramt;
        }

        private static string Decryption(string urlStr)
        {
            byte[] return64 = Convert.FromBase64String(urlStr);
            return Encoding.UTF8.GetString(return64);
        }

        private static string Encryption(string binaryStr)
        {
            byte[] _byte = Encoding.GetEncoding("UTF-8").GetBytes(binaryStr);
            return Convert.ToBase64String(_byte);
        }
        #endregion
    }
}
