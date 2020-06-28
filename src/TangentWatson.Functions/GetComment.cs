using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TangentWatson.Datastore.Interfaces;
using System.Text.Json;
using TangentWatson.Entities;
using TangentWatson.Settings;
using AppConstants = TangentWatson.Constants.Constants;

namespace TangentWatson.Functions
{
    public class GetComment
    {
        private readonly ICommentDataStoreService _commentDataStoreService;
        private readonly JsonSerializerOptions _serializerOptions;
        public GetComment(ICommentDataStoreService commentDataStoreService)
        {
            _commentDataStoreService = commentDataStoreService;
            _serializerOptions = Settngs.SerializerOptions;
        }

        /// <summary>
        /// Gets a comment and associated information
        /// </summary>
        /// <verb>GET</verb>
        /// <url>http://localhost:7071/api/Comment/{Id}</url>
        /// <param name="Id" in="path" cref="string">Comment ID to retrieve</param>
        /// <response code="200"><see cref="RatedComment"/>The comment and associated data</response>
        /// <response code="404">Item not found</response>
        /// <response code="400">Invalid guid supplied</response>
        [FunctionName("GetComment")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Comment/{Id}")] HttpRequest req, string Id,
            ILogger log)
        {
            log.LogDebug("Get Request for ID - {id}");
           if (Guid.TryParse(Id, out var docId))
           {
               var ratedComment = await _commentDataStoreService.GetMessage(docId);
               if (ratedComment == null)
               {
                   return new HttpResponseMessage
                   {
                       StatusCode = HttpStatusCode.NotFound,
                       Content = new StringContent($"Item with ID {Id} not found", Encoding.UTF8, AppConstants.TextResponse)
                   };
               }
               else
               {
                   var serializedComment = JsonSerializer.Serialize(ratedComment, _serializerOptions);
                   return new HttpResponseMessage
                   {
                       StatusCode = HttpStatusCode.OK,
                       Content = new StringContent(serializedComment, Encoding.UTF8, AppConstants.JsonResponse)
                   };
               }
           }
           else
           {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent($"The supplied ID - {Id} is not a valid object identifier", Encoding.UTF8, AppConstants.TextResponse)
                };
           }
        }
    }
}
