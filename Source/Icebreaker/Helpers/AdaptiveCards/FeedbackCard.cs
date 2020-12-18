using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Icebreaker.Components;
using Icebreaker.Properties;
using Microsoft.Bot.Connector.Teams.Models;

namespace Icebreaker.Helpers.AdaptiveCards
{
    public class FeedbackCard
    {
        /// <summary>
        /// Default marker string in the UPN that indicates a user is externally-authenticated
        /// </summary>
        private const string ExternallyAuthenticatedUpnMarker = "#ext#";

        private static readonly string CardTemplate;

        static FeedbackCard()
        {
            var cardJsonFilePath = HostingEnvironment.MapPath("~/Helpers/AdaptiveCards/FeedbackCard.json");
            CardTemplate = File.ReadAllText(cardJsonFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName1">Name of user, to who will send bot message</param>
        /// <param name="userName2">Name of user, with who user1 should meetup</param>
        /// <returns></returns>
        public static string GetCard(string userName1, string userName2)
        {
            var matchUpCardMatchedText = string.Format(Resources.MeetupFeedBackMsg, userName1, userName2);

            var variablesToValues = new Dictionary<string, string>()
            {
                { "mainMessageText", matchUpCardMatchedText },
                { "yesButtonText", Resources.Yes },
                { "notButtonText", Resources.No },
                { "yesTextName", CardActions.FeedbackYes},
                { "noTextName", CardActions.FeedbackNo}
            };

            var cardBody = CardTemplate;
            foreach (var kvp in variablesToValues)
            {
                cardBody = cardBody.Replace($"%{kvp.Key}%", kvp.Value);
            }

            return cardBody;
        }
    }
}