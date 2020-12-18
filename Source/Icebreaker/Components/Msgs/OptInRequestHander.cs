using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Icebreaker.Properties;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;

namespace Icebreaker.Components.IncomingMsgs
{
    public class OptInRequestHander : IRequestHandler<OptInRequest, Activity>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IcebreakerBot _bot;

        public OptInRequestHander(TelemetryClient telemetryClient, IcebreakerBot bot)
        {
            _telemetryClient = telemetryClient;
            _bot = bot;
        }

        public async Task<Activity> Handle(OptInRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;

            var senderAadId = activity.From.Properties["aadObjectId"].ToString();
            var tenantId = activity.GetChannelData<TeamsChannelData>().Tenant.Id;

            // User opted in
            _telemetryClient.TrackTrace($"User {senderAadId} opted in");

            var properties = new Dictionary<string, string>
            {
                {"UserAadId", senderAadId},
                {"OptInStatus", "true"},
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
                            Text = CardActions.Optout
                        }
                    }
                }.ToAttachment(),
            };
            return optInReply;
        }
    }
}