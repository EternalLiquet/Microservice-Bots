using System;
using MicroserviceBotsUtil.Entities;
using MicroserviceBotsUtil.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace PingFunction
{
    public static class ProcessMessages
    {
        [FunctionName("InboundMessageProcess")]
        [return: ServiceBus("pingqueue", Connection = "AzureWebJobsServiceBus")]
        public static string InboundMessageProcess([QueueTrigger("ping-inbound-queue")] CloudQueueMessage myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            ConvertedMessage message = DiscordConvert.DeSerializeObject(myQueueItem.AsString);

            if (message.Content.StartsWith("!ping"))
            {
                var returnMessage = new NewMessage();
                returnMessage.ChannelId = message.ChannelId;
                returnMessage.Content = "pong!";
                return JsonConvert.SerializeObject(returnMessage, Formatting.None);
            }
            return null;
        }
    }
}
