﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using StackExchange.Redis;

namespace ElastiCacheTest
{
    public class RequestSimulator
    {
        private readonly IDatabase _RedisDatabase;

        private readonly IAsyncPolicy _RedisBackoffPolicy;

        public RequestSimulator()
        {
            _RedisDatabase = GetRedisDatabaseConnection().Result;
            _RedisBackoffPolicy = InitializeRedisBackoffPolicy();
        }

        private IAsyncPolicy InitializeRedisBackoffPolicy()
        {
            return Policy
                .Handle<RedisServerException>()
                .Or<RedisCommandException>()
                .Or<RedisException>()
                .Or<RedisTimeoutException>()
                .WaitAndRetryAsync(
                    5,
                    (attemptCount, context) =>
                    {
                        return TimeSpan.FromSeconds(Math.Pow(2, attemptCount));
                    },
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Exception occured {exception.GetType()}, retry count {retryCount}, wait time {timeSpan}");
                    });
        }

        private async Task<IDatabase> GetRedisDatabaseConnection()
        {
            var redisEndpoints = new List<string>
            {
                "dentoncache2.zbumom.clustercfg.use2.cache.amazonaws.com:6379"
            };

            var configurationOptions = new ConfigurationOptions();
            redisEndpoints.ForEach(configurationOptions.EndPoints.Add);
            configurationOptions.ConnectTimeout = 60;
            configurationOptions.KeepAlive = 180;
            configurationOptions.ConfigCheckSeconds = 30;
            configurationOptions.AbortOnConnectFail = false;
            configurationOptions.ConnectRetry = 10;
            configurationOptions.ReconnectRetryPolicy = new ExponentialRetry(5000);
            configurationOptions.SyncTimeout = 60000;

            var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configurationOptions);
            return connectionMultiplexer.GetDatabase(0);
        }

        public async Task Run()
        {
            var requestInterval = TimeSpan.FromMilliseconds(500);

            while (true)
            {
                var redisKey = Guid.NewGuid().ToString();

                var setResult = await _RedisBackoffPolicy
                    .ExecuteAsync(() => _RedisDatabase.StringSetAsync(redisKey, "test-value"));
                Console.WriteLine($"Result for setting {redisKey}: {setResult}");
                var getResult = await _RedisBackoffPolicy
                    .ExecuteAsync(() => _RedisDatabase.StringGetAsync(redisKey));
                Console.WriteLine($"Result for getting {redisKey}: {getResult}");

                await Task.Delay(requestInterval);
            }
        }
    }
}