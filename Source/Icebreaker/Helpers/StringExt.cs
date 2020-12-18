using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icebreaker.Helpers
{
    public static class StringExt
    {
        public static bool Is(this string text, string value)
        {
            return string.Equals(text, value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}