using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AppConstants = TangentWatson.Constants.Constants;

namespace TangentWatson.Settings
{

    public static class Settngs
    {
        private static Lazy<JsonSerializerOptions> _jsonSerializerOptions;

        public static string DatabaseConnectionString =>
            Environment.GetEnvironmentVariable(AppConstants.DBConnectionSettingKey);

        public static string WatsonUrl => Environment.GetEnvironmentVariable(AppConstants.WatsonUrlKey);

        public static string WatsonApiKey => Environment.GetEnvironmentVariable(AppConstants.WatsonApiKeyKey);
        public static string WatsonUserName => AppConstants.WatsonAPIUserName;
        public static string RedisConnection => Environment.GetEnvironmentVariable(AppConstants.RedisConnectionKey);
        public static JsonSerializerOptions SerializerOptions
        {
            get
            {
                if (_jsonSerializerOptions == null)
                {
                    _jsonSerializerOptions = new Lazy<JsonSerializerOptions>(() =>
                    {
                        var options = new JsonSerializerOptions();
                        options.Converters.Add(new JsonStringEnumConverter());
                        return options;
                    });
                }

                return _jsonSerializerOptions.Value;
            }
        }

        
    }
}
