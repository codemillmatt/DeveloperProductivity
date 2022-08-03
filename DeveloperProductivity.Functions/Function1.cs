using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DeveloperProductivity.Functions
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([QueueTrigger("bad-weather-queue", Connection = "badweather-connection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
