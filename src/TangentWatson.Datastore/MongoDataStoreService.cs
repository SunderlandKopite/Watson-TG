using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using TangentWatson.Datastore.Interfaces;
using TangentWatson.Entities;
using TangentWatson.Settings;
using AppConstants = TangentWatson.Constants.Constants;
namespace TangentWatson.Datastore
{
    internal class MongoDataStoreService : ICommentDataStoreService
    {
        private readonly Lazy<IMongoCollection<RatedComment>> _mongoCollection;
        private readonly ILogger<MongoDataStoreService> _logger;

        public MongoDataStoreService(ILogger<MongoDataStoreService> logger, Lazy<IMongoCollection<RatedComment>> mongoCollection)
        {
            _mongoCollection = mongoCollection;
            _logger = logger;
        }

        public virtual async Task<RatedComment> GetMessage(Guid id, bool forUpdate)
        {
            var filter = Builders<RatedComment>.Filter.Eq(AppConstants.DbKeyField, id);
            var dbDoc =  await _mongoCollection.Value.Find(filter).SingleOrDefaultAsync();
            _logger.LogDebug(dbDoc==null?$"Null document returned from Mongo ID {id}":$"Document found in Mongo {id}");
            return dbDoc;
        }

        public virtual async Task<RatedComment> CreateAndStoreMessage(string message)
        {
            
            var ratedMessage = new RatedComment()
            {
               Message = message
            };

            await _mongoCollection.Value.InsertOneAsync(ratedMessage);
            _logger.LogInformation($"Uploaded comment stored in Mongo ID - {ratedMessage.UniqueID}");
            return ratedMessage;
        }

        public virtual async Task UpdateAsFailure(RatedComment comment)
        {
            var filter = Builders<RatedComment>.Filter.Eq(AppConstants.DbKeyField, comment.UniqueID);
            var updates = Builders<RatedComment>.Update.Combine(new[]
            {
                Builders<RatedComment>.Update.Set(AppConstants.WatsonStatusField, ResponseStatus.Failed)
            });
            await _mongoCollection.Value.UpdateOneAsync(filter, updates);
            _logger.LogInformation($"Document ID - {comment.UniqueID}, updated as failure");
        }

        public virtual async Task UpdateWithResults(RatedComment comment, WatsonResponse watsonResponse)
        {
            var filter = Builders<RatedComment>.Filter.Eq(AppConstants.DbKeyField, comment.UniqueID);
            var updates = Builders<RatedComment>.Update.Combine(new[]
            {
                Builders<RatedComment>.Update.Set(AppConstants.WatsonResponseField, watsonResponse),
                Builders<RatedComment>.Update.Set(AppConstants.WatsonStatusField, ResponseStatus.Complete)
            });
            await _mongoCollection.Value.UpdateOneAsync(filter, updates);
            _logger.LogInformation($"Document ID - {comment.UniqueID}, updated with watson results");
        }
    }
}
