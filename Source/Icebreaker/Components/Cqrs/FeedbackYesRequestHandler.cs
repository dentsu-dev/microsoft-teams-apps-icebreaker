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
    public class FeedbackYesRequestHandler : IRequestHandler<FeedbackYesRequest, Activity>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IcebreakerBot _bot;

        public FeedbackYesRequestHandler(TelemetryClient telemetryClient, IcebreakerBot bot)
        {
            _telemetryClient = telemetryClient;
            _bot = bot;
        }

        public async Task<Activity> Handle(FeedbackYesRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;

            var senderAadId = activity.From.Properties["aadObjectId"].ToString();
            var tenantId = activity.GetChannelData<TeamsChannelData>().Tenant.Id;

            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedBackAnswerText,
                    Buttons = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Title = Resources.FeedBackAnswerVeryGood,
                            DisplayText = Resources.FeedBackAnswerVeryGood,
                            Type = ActionTypes.MessageBack,
                            Text = "feedbackverygood"
                        },
                        new CardAction()
                        {
                            Title = Resources.FeedBackAnswerGood,
                            DisplayText = Resources.FeedBackAnswerGood,
                            Type = ActionTypes.MessageBack,
                            Text = "feedbackgood"
                        },
                        new CardAction()
                        {
                            Title = Resources.FeedBackAnswerNormal,
                            DisplayText = Resources.FeedBackAnswerNormal,
                            Type = ActionTypes.MessageBack,
                            Text = "feedbacknormal"
                        },
                        new CardAction()
                        {
                            Title = Resources.FeedBackAnswerBad,
                            DisplayText = Resources.FeedBackAnswerBad,
                            Type = ActionTypes.MessageBack,
                            Text = "feedbackbad"
                        }
                    }
                }.ToAttachment(),
            };

            return reply;
        }
    }
}