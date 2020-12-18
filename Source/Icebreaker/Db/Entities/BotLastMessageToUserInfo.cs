using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Icebreaker.Db.Entities
{
    /// <summary>
    /// Last message from bot to concrete user
    /// </summary>
    public class BotLastMessageInfo : Document
    {
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }
    }
}