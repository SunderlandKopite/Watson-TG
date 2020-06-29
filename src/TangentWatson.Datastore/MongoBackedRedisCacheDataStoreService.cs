using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StackExchange.Redis;
using TangentWatson.Datastore.Interfaces;
using TangentWatson.Entities;
using TangentWatson.Settings;

namespace TangentWatson.Datastore
{
    class MongoBackedRedisCacheDataStoreService : MongoDataStoreService
    {
        private readonly ILogger<MongoBackedRedisCacheDataStoreService> _logger;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        public MongoBackedRedisCacheDataStoreService(ILogger<MongoBackedRedisCacheDataStoreService> logger, Lazy<ConnectionMultiplexer> connectionMultiplexer, Lazy<IMongoCollection<RatedComment>> mongoCollection) :
            base(logger, mongoCollection)
        {
            _logger = logger;
            _connectionMultiplexer = connectionMultiplexer;
        }

        private IDatabase Database => _connectionMultiplexer.Value.GetDatabase();

        private async Task<RatedComment> GetRatedMessageFromCache(Guid id)
        {
            try
            {
                var cacheResult = await Database.StringGetAsync(new RedisKey(id.ToString()));
                if (!cacheResult.IsNull)
                {
                    _logger.LogInformation($"Value for ID {id} found in cache");
                    return JsonSerializer.Deserialize<RatedComment>(cacheResult.ToString());
                }

                return null;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occured reading from cache");
                return null;
            }
        }

        private async Task AddToCache(RatedComment ratedComment)
        {
            try
            {
                _logger.LogInformation($"Message id {ratedComment.UniqueID} added to cache");
                await Database.StringSetAsync(ratedComment.UniqueID.ToString(), JsonSerializer.Serialize(ratedComment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured reading from cache");
            }
        }

        private async Task ClearFromCache(RatedComment ratedComment)
        {
            try
            {
                await Database.KeyDeleteAsync(ratedComment.UniqueID.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing item from cache");
            }
        }
        public override async Task<RatedComment> GetMessage(Guid id, bool forUpdate = false)
        {
            var cacheResult = await GetRatedMessageFromCache(id);
            if (cacheResult != null)
            {
                return cacheResult;
            }
            var databaseResult = await base.GetMessage(id, forUpdate);
            if (databaseResult != null && !forUpdate)
            {
                await AddToCache(databaseResult);
            }

            return databaseResult;
        }

       
        public override async Task UpdateAsFailure(RatedComment comment)
        {
            var cacheClearTask = ClearFromCache(comment);
            var dbUpdateTask = base.UpdateAsFailure(comment);
            _logger.LogInformation($"Message id {comment.UniqueID} updating flushing from cache");
            await Task.WhenAll(cacheClearTask, dbUpdateTask);
        }

        public override async Task UpdateWithResults(RatedComment comment, WatsonResponse watsonResponse)
        {
            var cacheClearTask = ClearFromCache(comment);
            var dbUpdateTask = base.UpdateWithResults(comment, watsonResponse);
            _logger.LogInformation($"Message id {comment.UniqueID} updating flushing from cache");
            await Task.WhenAll(cacheClearTask, dbUpdateTask);
        }
    }
}
