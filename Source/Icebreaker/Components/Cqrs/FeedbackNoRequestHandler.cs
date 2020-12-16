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

namespace Icebreaker.Components.Cqrs
{
    public class FeedbackNoRequestHandler : IRequestHandler<FeedbackNoRequest,Activity>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IcebreakerBot _bot;

        public FeedbackNoRequestHandler(TelemetryClient telemetryClient, IcebreakerBot bot)
        {
            _telemetryClient = telemetryClient;
            _bot = bot;
        }

        public async Task<Activity> Handle(FeedbackNoRequest request, CancellationToken cancellationToken)
        {
            var reply = request.Activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedBackAnswerWhyNotText,
                    Buttons = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Title = Resources.FeedBackAnswerNoResponse,
                            DisplayText = Resources.FeedBackAnswerNoResponse,
                            Type = ActionTypes.MessageBack,
                            Text = "feedbacknoresponse"
                        },
                        new CardAction()
                        {
                            Title = Resources.FeedBackAnswerCanceled,
                            DisplayText = Resources.FeedBackAnswerCanceled,
                            Type = ActionTypes.MessageBack,
                            Text = "feedbackcanceled"
                        },
                        new CardAction()
                        {
                            Title = Resources.FeedBackAnswerNoGoodTime,
                            DisplayText = Resources.FeedBackAnswerNoGoodTime,
                            Type = ActionTypes.MessageBack,
                            Text = "feedbacknogoodtime"
                        }
                    }
                }.ToAttachment(),
            };

            return reply;
        }
    }
}