using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalileuszSchool.External
{
    public class FacebookTokenValidationResult
    {
        [JsonProperty("data")]
        public FacebookTokenValidationResultData Data { get; set; }
    }

    public class FacebookTokenValidationResultData
    {
        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("application")]
        public string Application { get; set; }

        [JsonProperty("data_access_expires_at")]
        public long DataAccessExpiresAt { get; set; }

        [JsonProperty("expires_at")]
        public long ExpiresAt { get; set; }

        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }

        [JsonProperty("scopes")]
        public string[] Scopes { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
