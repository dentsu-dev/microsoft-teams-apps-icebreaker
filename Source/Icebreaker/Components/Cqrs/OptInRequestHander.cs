using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Icebreaker.Properties;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;

namespace Icebreaker.Components.Cqrs
{
    public class OptInRequestHander : IRequestHandler<OptInRequest>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IcebreakerBot _bot;

        public OptInRequestHander(TelemetryClient telemetryClient, IcebreakerBot bot)
        {
            _telemetryClient = telemetryClient;
            _bot = bot;
        }

        public async Task<Unit> Handle(OptInRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;
            var connectorClient = request.connectorClient;

            var senderAadId = activity.From.Properties["aadObjectId"].ToString();
            var tenantId = activity.GetChannelData<TeamsChannelData>().Tenant.Id;

            // User opted in
            _telemetryClient.TrackTrace($"User {senderAadId} opted in");

            var properties = new Dictionary<string, string>
            {
                { "UserAadId", senderAadId },
                { "OptInStatus", "true" },
            };
            _telemetryClient.TrackEvent("UserOptInStatusSet", properties);

            await _bot.OptInUser(tenantId, senderAadId, activity.ServiceUrl);

            var optInReply = activity.CreateReply();
            optInReply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.OptInConfirmation,
                    Buttons = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Title = Resources.PausePairingsButtonText,
                            DisplayText = Resources.PausePairingsButtonText,
                            Type = ActionTypes.MessageBack,
                            Text = "optout"
                        }
                    }
                }.ToAttachment(),
            };

            await connectorClient.Conversations.ReplyToActivityAsync(optInReply,cancellationToken);

            return Unit.Value;
        }
    }
}