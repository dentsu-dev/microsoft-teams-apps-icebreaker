using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Icebreaker.Helpers.Db
{
    public class UserMatchInfo : Document
    {
        /// <summary>
        /// Identicator to MsTeamUserId
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the tenant id
        /// </summary>
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the service URL
        /// </summary>
        [JsonProperty("serviceUrl")]
        public string ServiceUrl { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("senderGivenName")]
        public string SenderGivenName { get; set; }

        [JsonProperty("recipientGivenName")]
        public string RecipientGivenName { get; set; }

        [JsonProperty("senderEmail")]
        public string SenderEmail { get; set; }

        [JsonProperty("recipientEmail")]
        public string RecipientEmail { get; set; }
    }
}