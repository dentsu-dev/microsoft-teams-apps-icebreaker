using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
#pragma warning disable 1591

namespace Icebreaker.Components
{
    public class BotMessageTypes
    {
        /// <summary>
        /// sent new math to meetup
        /// </summary>
        public static string NewMatchedPair = "newMatcherPair";

        /// <summary>
        /// request feedback after last meetup
        /// </summary>
        public static string Fb = "fb";

        /// <summary>
        /// bot sent optOut message to user
        /// </summary>
        public static string OptOut = "optOut";

        /// <summary>
        /// bot sent optIn message to user
        /// </summary>
        public static string OptIn = "optIn";

        /// <summary>
        /// requested details after answer "Yes"
        /// </summary>
        public static string FbRootYesDetails = "fbRootYesDetails";

        /// <summary>
        /// request details after answer "No" 
        /// </summary>
        public static string FbRootNoDetails = "fbRootNoDetails";

        /// <summary>
        /// user sent unknown answer, bot requested to choose answer again
        /// </summary>
        public static string FbRootUnknownAnswer = "fbRootUnknownAnswer";

        /// <summary>
        /// user sent great response, bot sent response
        /// </summary>
        public static string FbYesSuperResponse = "fbYesSuperResponse";

        /// <summary>
        /// user sent good response, bot sent response
        /// </summary>
        public static string FbYesGoodResponse = "fbYesGoodResponse";

        /// <summary>
        /// user sent bad response, bot sent response
        /// </summary>
        public static string FbYesBadResponse = "fbYesBadResponse";

        /// <summary>
        /// user sent unknown answer, bot bot assumed that it will be answer for YesDetails. Retry user to send known answer
        /// </summary>
        public static string FbYesUnknownAnswer = "fbYesUnknownAnswer";

        /// <summary>
        /// user sent no meetup, bot sent response
        /// </summary>
        public static string FbNoMeetupResponse = "fbNoMeetupResponse";

        /// <summary>
        /// user sent canceled, bot sent response
        /// </summary>
        public static string FbNoCanceledResponse = "fbCanceledResponse";

        /// <summary>
        /// user sent not good time, bot send response
        /// </summary>
        public static string FbNoGoodTimeResponse = "fbNoGoodTimeResponse";

        /// <summary>
        /// user sent another reason, bot send response
        /// </summary>
        public static string FbNoAnotherReasonResponse = "fbNoAnotherReasonResponse";

        /// <summary>
        /// user send unknown answer, bot assumed that it will be answer of NoDetails, bot send message about it
        /// </summary>
        public static string FbNoUnknownAnswerResponse = "fbNoUnknownAnswerResponse";


        /// <summary>
        /// bot accepted comments after answer "great" and sent response to user
        /// </summary>
        public static string FbSuperCommentsAccepted = "fbSuperCommentsAccepted";

        /// <summary>
        /// bot accepted comments after answer "good" and sent response to user
        /// </summary>
        public static string FbGoodCommentsAccepted = "fbGoodCommentsAccepted";

        /// <summary>
        /// bot accepted comment after answer "bad" and sent response to user
        /// </summary>
        public static string FbBadCommentsAccepted = "fbCommentsAccepted";

        /// <summary>
        /// bot accepted comment after answer "no good time" and sent response to user
        /// </summary>
        public static string FbNoGoodTimeCommentsAccepted = "fbNoGoodTimeCommentsAccepted";

        /// <summary>
        /// bot accepted comments after answer "canceled" and sent response to user
        /// </summary>
        public static string FbCanceledCommentsAccepted = "fbCanceledCommentsAccepted";

        /// <summary>
        /// bot accepted comments after answer "another reason" and sent response to user
        /// </summary>
        public static string FbAnotherReasonCommentsAccepted = "fbAnotherReasonCommentsAccepted";

        /// <summary>
        /// Feedback scenario completed, user sent any request, bot has no answer and sent message about it to user
        /// </summary>
        public static string ByeForNextPeriod = "byeForNextPeriod";
    }
}