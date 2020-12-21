using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Icebreaker.Components.Msgs;
using Icebreaker.Db;
using Icebreaker.Helpers;
using Icebreaker.Helpers.AdaptiveCards;
using Icebreaker.Properties;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Newtonsoft.Json;

namespace Icebreaker.Components.IncomingMsgs
{
    public class CommonTextRequestHandler : IRequestHandler<CommonTextRequest, CommonTextResult>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IcebreakerBot _bot;
        private readonly BotRepository _repository;

        public CommonTextRequestHandler(TelemetryClient telemetryClient, IcebreakerBot bot, BotRepository repository)
        {
            _telemetryClient = telemetryClient;
            _bot = bot;
            _repository = repository;
        }

        public async Task<CommonTextResult> Handle(CommonTextRequest request, CancellationToken cancellationToken)
        {
            // Unknown input
            _telemetryClient.TrackTrace($"Handling common message from user: {request.Activity.Text}");

            var activity = request.Activity;
            var botMsg = request.BotLastMessage.Message;
            var lastMatch = request.UserMatch;

            var replyActivity = request.Activity.CreateReply();
            string newbotMessage;

            if (botMsg.Is(BotMessageTypes.FbYesSuperResponse))
            {
                await _repository.FeedbackCommentCreate(
                    lastMatch.SenderEmail,
                    lastMatch.SenderAadId,
                    lastMatch.RecipientEmail,
                    lastMatch.RecipientAadId,
                    FbDetailTypes.Super,
                    FbRootTypes.Yes,
                    activity.Text);
                ReplyAfterComment(replyActivity);
                newbotMessage = BotMessageTypes.FbSuperCommentsAccepted;
            }
            else if (botMsg.Is(BotMessageTypes.FbYesGoodResponse))
            {
                await _repository.FeedbackCommentCreate(
                    lastMatch.SenderEmail,
                    lastMatch.SenderAadId,
                    lastMatch.RecipientEmail,
                    lastMatch.RecipientAadId,
                    FbDetailTypes.Good,
                    FbRootTypes.Yes,
                    activity.Text);
                ReplyAfterComment(replyActivity);
                newbotMessage = BotMessageTypes.FbGoodCommentsAccepted;
            }
            else if (botMsg.Is(BotMessageTypes.FbYesBadResponse))
            {
                await _repository.FeedbackCommentCreate(
                    lastMatch.SenderEmail,
                    lastMatch.SenderAadId,
                    lastMatch.RecipientEmail,
                    lastMatch.RecipientAadId,
                    FbDetailTypes.Bad,
                    FbRootTypes.Yes,
                    activity.Text);
                ReplyAfterComment(replyActivity);
                newbotMessage = BotMessageTypes.FbBadCommentsAccepted;
            }
            else if (botMsg.Is(BotMessageTypes.FbNoCanceledResponse))
            {
                await _repository.FeedbackCommentCreate(
                    lastMatch.SenderEmail,
                    lastMatch.SenderAadId,
                    lastMatch.RecipientEmail,
                    lastMatch.RecipientAadId,
                    FbDetailTypes.Canceled,
                    FbRootTypes.No,
                    activity.Text);
                ReplyAfterComment(replyActivity);
                newbotMessage = BotMessageTypes.FbCanceledCommentsAccepted;
            }
            else if (botMsg.Is(BotMessageTypes.FbNoGoodTimeResponse))
            {
                await _repository.FeedbackCommentCreate(
                    lastMatch.SenderEmail,
                    lastMatch.SenderAadId,
                    lastMatch.RecipientEmail,
                    lastMatch.RecipientAadId,
                    FbDetailTypes.NoGoodTime,
                    FbRootTypes.No,
                    activity.Text);
                ReplyAfterComment(replyActivity);
                newbotMessage = BotMessageTypes.FbNoGoodTimeCommentsAccepted;
            }
            else if (botMsg.Is(BotMessageTypes.FbNoAnotherReasonResponse))
            {
                await _repository.FeedbackCommentCreate(
                    lastMatch.SenderEmail,
                    lastMatch.SenderAadId,
                    lastMatch.RecipientEmail,
                    lastMatch.RecipientAadId,
                    FbDetailTypes.NoAnother,
                    FbRootTypes.No,
                    activity.Text);
                ReplyAfterComment(replyActivity);
                newbotMessage = BotMessageTypes.FbAnotherReasonCommentsAccepted;
            }
            else
            {
                if (botMsg.Is(BotMessageTypes.Fb) ||
                    botMsg.Is(BotMessageTypes.FbRootYesDetails) ||
                    botMsg.Is(BotMessageTypes.FbRootNoDetails) ||
                    botMsg.Is(BotMessageTypes.FbRootUnknownAnswer) ||
                    botMsg.Is(BotMessageTypes.FbYesUnknownAnswer) ||
                    botMsg.Is(BotMessageTypes.FbNoUnknownAnswerResponse))
                {
                    ReplyUnknownChooseAnswer(replyActivity);
                    if (botMsg.Is(BotMessageTypes.Fb))
                    {
                        newbotMessage = BotMessageTypes.FbRootUnknownAnswer;
                    }
                    else if (botMsg.Is(BotMessageTypes.FbRootYesDetails))
                    {
                        newbotMessage = BotMessageTypes.FbYesUnknownAnswer;
                    }
                    else if (botMsg.Is(BotMessageTypes.FbRootNoDetails))
                    {
                        newbotMessage = BotMessageTypes.FbNoUnknownAnswerResponse;
                    }
                    else
                    {
                        newbotMessage = botMsg;
                    }
                }
                else
                {
                    await _repository.UnknownMessageCreate(lastMatch.SenderEmail, lastMatch.SenderAadId, activity.Text);
                    ReplyBye(replyActivity);
                    newbotMessage = BotMessageTypes.ByeForNextPeriod;
                }
            }

            return new CommonTextResult
            {
                Reply = replyActivity,
                NewBotMessage = newbotMessage
            };

            //var unrecognizedInputAdaptiveCard = UnrecognizedInputAdaptiveCard.GetCard();
            //replyActivity.Attachments = new List<Attachment>()
            //{
            //    new Attachment()
            //    {
            //        ContentType = "application/vnd.microsoft.card.adaptive",
            //        Content = JsonConvert.DeserializeObject(unrecognizedInputAdaptiveCard)
            //    }
            //};
        }

        private void ReplyAfterComment(Activity replyActivity)
        {
            replyActivity.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedbackReplyAfterCommentText
                }.ToAttachment(),
            };
        }

        private void ReplyUnknownChooseAnswer(Activity replyActivity)
        {
            replyActivity.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.UnknownChooseAnswerText
                }.ToAttachment(),
            };
        }

        private void ReplyBye(Activity replyActivity)
        {
            replyActivity.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.ByeReplyText
                }.ToAttachment(),
            };
        }
    }
}