﻿using System;
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
            _Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json")
                .Build();

            Run().Wait();
        }

        private static async Task Run()
        {
            var requestSimulator = new RequestSimulator();
            var requestSimulatorTask = requestSimulator.Run();

            //var awsOptions = _Configuration.GetAWSOptions();
            //var amazonElastiCache = awsOptions.CreateServiceClient<IAmazonElastiCache>();

            //var elastiCacheFailoverSimulator = new ElastiCacheFailoverSimulator(amazonElastiCache);
            //var elastiCacheFailoverSimulatorTask = elastiCacheFailoverSimulator.Run();

            await Task.WhenAll(
                //elastiCacheFailoverSimulatorTask,
                requestSimulatorTask);
        }
    }
}
