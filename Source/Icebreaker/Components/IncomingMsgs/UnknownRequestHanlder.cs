using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Icebreaker.Helpers.AdaptiveCards;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Icebreaker.Components.IncomingMsgs
{
    public class UnknownRequestHandler : IRequestHandler<UnknownRequest, Activity>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IcebreakerBot _bot;

        public UnknownRequestHandler(TelemetryClient telemetryClient, IcebreakerBot bot)
        {
            _telemetryClient = telemetryClient;
            _bot = bot;
        }

        public async Task<Activity> Handle(UnknownRequest request, CancellationToken cancellationToken)
        {
            // Unknown input
            _telemetryClient.TrackTrace($"Cannot process the following: {request.Activity.Text}");
            var replyActivity = request.Activity.CreateReply();

            var unrecognizedInputAdaptiveCard = UnrecognizedInputAdaptiveCard.GetCard();
            replyActivity.Attachments = new List<Attachment>()
            {
                new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(unrecognizedInputAdaptiveCard)
                }
            };

            return replyActivity;
        }
    }
}