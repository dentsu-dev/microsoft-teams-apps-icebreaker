using System;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Icebreaker.Db.Entities
{
    public class UserMatchInfo : Document
    {
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("senderEmail")]
        public string SenderEmail { get; set; }

        [JsonProperty("senderAadId")]
        public string SenderAadId { get; set; }

        [JsonProperty("recipientEmail")]
        public string RecipientEmail { get; set; }

        [JsonProperty("recipientAadId")]
        public string RecipientAadId { get; set; }
    }
}