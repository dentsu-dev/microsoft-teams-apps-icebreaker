using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Icebreaker.Helpers;
using Icebreaker.Properties;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;

namespace Icebreaker.Components.IncomingMsgs
{
    public class OptOutRequestHandler : IRequestHandler<OptOutRequest,Activity>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IcebreakerBot _bot;

        public OptOutRequestHandler(TelemetryClient telemetryClient, IcebreakerBot bot)
        {
            _telemetryClient = telemetryClient;
            _bot = bot;
        }

        public async Task<Activity> Handle(OptOutRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;

            var senderAadId = activity.SenderAadId();
            var tenantId = activity.GetChannelData<TeamsChannelData>().Tenant.Id;


            // User opted out
            _telemetryClient.TrackTrace($"User {senderAadId} opted out");

            var properties = new Dictionary<string, string>
            {
                {"UserAadId", senderAadId},
                {"OptInStatus", "false"},
            };
            _telemetryClient.TrackEvent("UserOptInStausSet", properties);

            await _bot.OptOutUser(tenantId, senderAadId, activity.ServiceUrl);

            var optOutReply = activity.CreateReply();
            optOutReply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.OptOutConfirmation,
                    Buttons = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Title = Resources.ResumePairingsButtonText,
                            DisplayText = Resources.ResumePairingsButtonText,
                            Type = ActionTypes.MessageBack,
                            Text = CardActions.Optin
                        }
                    }
                }.ToAttachment(),
            };

            return optOutReply;
        }
    }
}