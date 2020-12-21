using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;

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
            return activity.From.Properties["aadObjectId"].ToString();
        }
    }
}