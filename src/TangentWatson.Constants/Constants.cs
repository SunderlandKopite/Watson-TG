using System;

namespace TangentWatson.Constants
{
    public class Constants
    {
        public const string WatsonAPIUserName = "apikey";
        public const string WatsonUrlKey = "WatsonUrl";
        public const string WatsonApiKeyKey = "WatsonAPIKey";

        public const string DBConnectionSettingKey = "DatabaseConnection";
        public const string DbKeyField = "UniqueID";
        public const string DbName = "TangentWatson";
        public const string CollectionName = "Comments";
        public const string WatsonResponseField = "WatsonResponse";
        public const string WatsonStatusField = "Status";

        public const string RedisConnectionKey = "RedisConnection";

        public const string JsonResponse = "application/json";
        public const string TextResponse = "text/plain";
    }
}
