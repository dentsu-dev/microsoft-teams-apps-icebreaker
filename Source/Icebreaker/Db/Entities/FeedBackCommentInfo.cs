using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Icebreaker.Db.Entities
{
    public class FeedBackCommentInfo : Document
    {
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("companionUserEmail")]
        public string CompanionUserEmail { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("rootType")]
        public string RootType { get; set; }

        [JsonProperty("detailType")]
        public string DetailType { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}