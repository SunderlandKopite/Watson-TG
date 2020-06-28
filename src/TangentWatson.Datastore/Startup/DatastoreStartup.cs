using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StackExchange.Redis;
using TangentWatson.Datastore.Interfaces;
using TangentWatson.Datastore.Startup;
using TangentWatson.Entities;
using TangentWatson.Settings;


[assembly: FunctionsStartup(typeof(DatastoreStartup))]
namespace TangentWatson.Datastore.Startup
{
    class DatastoreStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var mongoCollection = new Lazy<IMongoCollection<RatedComment>>(() =>
            {
                var connectionString = Settngs.DatabaseConnectionString;
                var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                var mongoClient = new MongoClient(settings);
                var db = mongoClient.GetDatabase(Constants.Constants.DbName);
                return db.GetCollection<RatedComment>(Constants.Constants.CollectionName);
            });

            var multiplexer = new Lazy<ConnectionMultiplexer>(() =>
            {
                var connectionString = Settngs.RedisConnection;
                return ConnectionMultiplexer.Connect(connectionString);
            });

            builder.Services.AddSingleton(mongoCollection);
            builder.Services.AddSingleton(multiplexer);

            if (Environment.GetEnvironmentVariable("UseCachedDBImplementation")
                .Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddSingleton<ICommentDataStoreService, MongoBackedRedisCacheDataStoreService>(
                    x => ActivatorUtilities.CreateInstance<MongoBackedRedisCacheDataStoreService>(x, multiplexer, mongoCollection));
               // builder.Services.AddSingleton<ICommentDataStoreService, MongoBackedRedisCacheDataStoreService>(x=>new MongoBackedRedisCacheDataStoreService(x.GetRequiredService<ILogger<MongoBackedRedisCacheDataStoreService>>(), multiplexer, mongoCollection));
            }
            else
            {
                builder.Services.AddSingleton<ICommentDataStoreService, MongoDataStoreService>(x =>
                    ActivatorUtilities.CreateInstance<MongoDataStoreService>(x, mongoCollection));
               // builder.Services.AddSingleton<ICommentDataStoreService, MongoDataStoreService>(x=> new MongoDataStoreService(x.GetRequiredService<ILogger<MongoDataStoreService>>(), mongoCollection));
            }
        }


    }
}
