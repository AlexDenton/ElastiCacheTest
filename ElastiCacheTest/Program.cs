using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.ElastiCache;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace ElastiCacheTest
{
    class Program
    {
        private static IConfiguration _Configuration;

        static void Main(string[] args)
        {
            //_Configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appSettings.json")
            //    .Build();

            Run().Wait();
        }

        private static async Task Run()
        {
            var redisEndpoints = new List<string>
            {
                "dentoncache-001.zbumom.0001.use2.cache.amazonaws.com:6379",
                "dentoncache-002.zbumom.0001.use2.cache.amazonaws.com:6379",
                "dentoncache-003.zbumom.0001.use2.cache.amazonaws.com:6379"
            };

            var configurationOptions = new ConfigurationOptions();
            redisEndpoints.ForEach(configurationOptions.EndPoints.Add);
            configurationOptions.KeepAlive = 180;
            configurationOptions.ConfigCheckSeconds = 30;
            configurationOptions.AbortOnConnectFail = false;

            var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configurationOptions);
            var database = connectionMultiplexer.GetDatabase(0);
            var setResult = database.StringSet("test-key", "test-value");
            Console.WriteLine(setResult);
            var getResult = database.StringGet("test-key");
            Console.WriteLine(getResult.ToString());
        }
    }
}
