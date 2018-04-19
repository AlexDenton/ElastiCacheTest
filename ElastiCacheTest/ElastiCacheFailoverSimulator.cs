using System;
using System.Threading.Tasks;
using Amazon.ElastiCache;
using Amazon.ElastiCache.Model;

namespace ElastiCacheTest
{
    public class ElastiCacheFailoverSimulator
    {
        private readonly IAmazonElastiCache _AmazonElastiCache;

        public ElastiCacheFailoverSimulator(IAmazonElastiCache amazonElastiCache)
        {
            _AmazonElastiCache = amazonElastiCache;
        }

        public async Task Run()
        {
            var failoverInterval = TimeSpan.FromSeconds(3);
            await Task.Delay(failoverInterval);

            var testFailoverRequest = new TestFailoverRequest
            {
                ReplicationGroupId = "dentoncache",
                NodeGroupId = "dentoncache-001"
            };

            Console.WriteLine("Failing over...");
            var testFailoverResponse = await _AmazonElastiCache.TestFailoverAsync(testFailoverRequest);
            Console.WriteLine($"Response status code {testFailoverResponse.HttpStatusCode}");
        }
    }
}