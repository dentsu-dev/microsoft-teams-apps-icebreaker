using System;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Icebreaker.Db.Entities
{
    public class UserMatchInfo : Document
    {
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
        public DateTime Created { get; set; }

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