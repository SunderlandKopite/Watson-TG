using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TangentWatson.Datastore.Interfaces;

namespace TangentWatson.Functions
{
    public class DeadLetter
    {
        private readonly ICommentDataStoreService _commentDataStoreService;

        public DeadLetter(ICommentDataStoreService commentDataStoreService)
        {
            _commentDataStoreService = commentDataStoreService;
        }
        
        [FunctionName("DeadLetter")]
        public async Task Run([ServiceBusTrigger("pendingqueue/$DeadLetterQueue", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
        {
            //we dont want to fail here - just handle it
            if (Guid.TryParse(myQueueItem, out var Id))
            {
                var comment = await _commentDataStoreService.GetMessage(Id);
                if (comment != null)
                {
                   await  _commentDataStoreService.UpdateAsFailure(comment);
                }
            }
        }
    }
}
