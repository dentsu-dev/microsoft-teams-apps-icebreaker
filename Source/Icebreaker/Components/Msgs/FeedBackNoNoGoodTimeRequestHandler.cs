using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Icebreaker.Db;
using Icebreaker.Properties;
using MediatR;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;

namespace Icebreaker.Components.Msgs
{
    public class FeedBackNoNoGoodTimeRequestHandler : IRequestHandler<FeedBackNoNoGoodTimeRequest, Activity>
    {
        private readonly BotRepository _repository;

        public FeedBackNoNoGoodTimeRequestHandler(BotRepository repository)
        {
            _repository = repository;
        }

        public async Task<Activity> Handle(FeedBackNoNoGoodTimeRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;

            var reply = request.Activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedBackNoNoGoodTimeReplyText
                }.ToAttachment(),
            };

            var searchDate = DateTime.UtcNow.AddDays(-1 * Constants.FeedBackDelayDays);
            var lastUserMatch =
                await _repository.UserMatchInfoSearchByDateAndUser(
                    searchDate,
                    activity.From.AsTeamsChannelAccount().Email);

            var lastCompanionEmail = string.Empty;
            if (lastUserMatch != null)
            {
                lastCompanionEmail = lastUserMatch.RecipientEmail;
            }

            await _repository.FeedbackDetailCreate(
                activity.From.AsTeamsChannelAccount().Email,
                lastCompanionEmail,
                FbDetailTypes.NoGoodTime,
                FbRootTypes.No);

            return reply;
        }
    }
}