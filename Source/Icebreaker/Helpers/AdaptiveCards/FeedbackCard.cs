using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
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

        public static string GetCard(string userName)
        {
            var matchUpCardMatchedText = string.Format(Resources.MeetupFeedBackMsg, userName);

            var variablesToValues = new Dictionary<string, string>()
            {
                { "mainMessageText", matchUpCardMatchedText },
                { "yesButtonText", Resources.Yes },
                { "notButtonText", Resources.No }
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