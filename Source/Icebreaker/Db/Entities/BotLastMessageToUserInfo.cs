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
        /// <summary>
        /// Gets or sets the user's id in Teams (29:xxx).
        /// This is also the <see cref="Resource.Id"/>.
        /// </summary>
        [JsonIgnore]
        public string UserId
        {
            get { return this.Id; }
            set { this.Id = value; }
        }

        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }
    }
}