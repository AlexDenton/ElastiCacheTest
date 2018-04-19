using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ElastiCacheTest
{
    public class RequestSimulator
    {
        private readonly IDatabase _RedisDatabase;

        public RequestSimulator()
        {
            _RedisDatabase = GetRedisDatabaseConnection().Result;
        }

        private async Task<IDatabase> GetRedisDatabaseConnection()
        {
            var redisEndpoints = new List<string>
            {
                "dentoncache1.zbumom.clustercfg.use2.cache.amazonaws.com:6379"
            };

            var configurationOptions = new ConfigurationOptions();
            redisEndpoints.ForEach(configurationOptions.EndPoints.Add);
            configurationOptions.ConnectTimeout = 60;
            configurationOptions.KeepAlive = 180;
            configurationOptions.ConfigCheckSeconds = 30;
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.ConnectRetry = 10;
            configurationOptions.ReconnectRetryPolicy = new ExponentialRetry(5000);
            configurationOptions.SyncTimeout = 60;

            var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configurationOptions);
            return connectionMultiplexer.GetDatabase(0);
        }

        public async Task Run()
        {
            var requestInterval = TimeSpan.FromMilliseconds(500);

            while (true)
            {
                var redisKey = Guid.NewGuid().ToString();
                var setResult = _RedisDatabase.StringSet(redisKey, "test-value");
                Console.WriteLine($"Result for setting {redisKey}: {setResult}");
                var getResult = _RedisDatabase.StringGet(redisKey);
                Console.WriteLine($"Result for getting {redisKey}: {getResult}");

                await Task.Delay(requestInterval);
            }
        }
    }
}