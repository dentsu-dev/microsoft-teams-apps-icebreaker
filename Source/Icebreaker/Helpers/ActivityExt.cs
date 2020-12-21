using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Icebreaker.Helpers
{
    public static class ActivityExt
    {
        public static bool Is(this Activity activity, string name)
        {
            return string.Equals(activity.Text, name, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string SenderAadId(this Activity activity)
        {
            var json = JsonConvert.SerializeObject(activity);
            var jObject = JsonConvert.DeserializeObject<JObject>(json);

            var obj = jObject["from"]["aadObjectId"];
            return obj.Value<string>();
            //return activity.From.Properties["aadObjectId"].ToString();
        }
    }
}