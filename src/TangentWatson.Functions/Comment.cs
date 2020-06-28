using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using TangentWatson.Datastore.Interfaces;
using TangentWatson.Entities;
using TangentWatson.Settings;
using AppConstants = TangentWatson.Constants.Constants;

namespace TangentWatson.Functions
{
    public class AddComment
    {
        private readonly ICommentDataStoreService _commentDataStoreService;
        private readonly JsonSerializerOptions _serializerOptions;
        public AddComment(ICommentDataStoreService commentDataStoreService)
        {
            _commentDataStoreService = commentDataStoreService;
            _serializerOptions = Settngs.SerializerOptions;
        }

        /// <summary>
        /// Adds a comment to be processed
        /// </summary>
        /// <verb>POST</verb>
        /// <url>http://localhost:7071/api/Comment</url>
        /// <param name="postBody" in="body" cref="string">Comment to be analysed</param>
        /// <response code="200"><see cref="RatedComment"/>The message queued for watson</response>
        /// <response code="404">No comment specified</response>
        /// <response code="500">Internal server error</response>
        [FunctionName("Comment")]
      public async Task<HttpResponseMessage> Run(
          [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Comment")]
          HttpRequest req, [ServiceBus("pendingqueue", EntityType.Queue, Connection = "ServiceBusConnection")]
          ICollector<string> queueCollector,
          ILogger log)
      {
          var messageBody = await req.ReadAsStringAsync();
          if (messageBody != null && messageBody.Trim().Length > 0)
          {
              try
              {
                  log.LogInformation(
                      $"uploaded message starting {(messageBody.Length > 100 ? messageBody.Substring(0, 100) : messageBody)}");
                  var message = await _commentDataStoreService.CreateAndStoreMessage(messageBody);

                  queueCollector.Add(message.UniqueID.ToString());

                  return new HttpResponseMessage
                  {
                      StatusCode = HttpStatusCode.Accepted,
                      Content = new StringContent(JsonSerializer.Serialize(message, _serializerOptions), Encoding.UTF8,
                          AppConstants.JsonResponse)
                  };
              }
              catch (Exception ex)
              {
                  log.LogError("An error has occured logging the request.", ex);
                  return new HttpResponseMessage
                  {
                      StatusCode = HttpStatusCode.InternalServerError,
                      Content = new StringContent("Sorry - an internal error has occured", Encoding.UTF8,
                          AppConstants.TextResponse)
                  };
                }
          }
          else
          {
              log.LogInformation("Request with no comment received");
              return new HttpResponseMessage
              {
                  StatusCode = HttpStatusCode.BadRequest,
                  Content = new StringContent("Please ensure the post message contains the comment to rate", Encoding.UTF8,
                      AppConstants.TextResponse)
              };
            }
      }
    }
}
