using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Icebreaker.Db.Entities
{
    public class FeedBackDetailInfo : Document
    {
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("companionUserEmail")]
        public string CompanionUserEmail { get; set; }

        [JsonProperty("rootType")]
        public string RootType { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}