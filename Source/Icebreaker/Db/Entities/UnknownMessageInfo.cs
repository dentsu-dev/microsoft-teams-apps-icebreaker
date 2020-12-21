using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Icebreaker.Db.Entities
{
    public class UnknownMessageInfo : Document
    {
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("userAaId")]
        public string UserAadId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}