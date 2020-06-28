using System;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TangentWatson.Datastore.Interfaces;
using TangentWatson.WatsonClient.Interfaces;

namespace TangentWatson.Functions
{
    internal class WatsonMessageProcessor
    {

        private readonly ICommentDataStoreService _commentDataStoreService;
        private readonly IWatsonClient _watsonClient;

        public WatsonMessageProcessor(ICommentDataStoreService commentDataStoreService, IWatsonClient watsonClient)
        {
            _commentDataStoreService = commentDataStoreService;
            _watsonClient = watsonClient;
        }

        [FunctionName("WatsonMessageProcessor")]
        public async Task Run([ServiceBusTrigger("pendingqueue", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
        {
            log.LogDebug($"Processing item for ID - {myQueueItem}");
            var message = await _commentDataStoreService.GetMessage(Guid.Parse(myQueueItem), true);
            if (message == null)
            {
                throw new Exception($"Item not found in store {myQueueItem}");
            }

            var watsonResponse = await _watsonClient.GetResponse(message.Message);

            await _commentDataStoreService.UpdateWithResults(message, watsonResponse);
        }
    }
}
