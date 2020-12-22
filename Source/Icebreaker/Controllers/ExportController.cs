using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Icebreaker.Components;
using Icebreaker.Db;
using Microsoft.Azure;
using Microsoft.Bot.Connector;

namespace Icebreaker.Controllers
{
    public class ExportController : ApiController
    {
        private readonly IcebreakerBot bot;
        private readonly MicrosoftAppCredentials botCredentials;
        private readonly BotRepository _botRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessNowController"/> class.
        /// </summary>
        /// <param name="bot">The Icebreaker bot instance</param>
        /// <param name="botCredentials">The bot AAD credentials</param>
        public ExportController(IcebreakerBot bot, MicrosoftAppCredentials botCredentials, BotRepository botRepository)
        {
            this.bot = bot;
            this.botCredentials = botCredentials;
            _botRepository = botRepository;
        }

        /// <summary>
        /// Action to export out FeedBackRoot entities
        /// </summary>
        /// <param name="key">API key</param>
        /// <returns>content</returns>
        [Route("api/export/fbroot/{key}")]
        public async Task<IHttpActionResult> GetFeedbackRoot([FromUri] string key)
        {
            var isKeyMatch = object.Equals(key, CloudConfigurationManager.GetSetting("Key"));
            if (isKeyMatch)
            {
                // Get the token here to proactively trigger a refresh if the cached token is expired
                // This avoids a race condition in MicrosoftAppCredentials.GetTokenAsync that can lead it to return an expired token
                await this.botCredentials.GetTokenAsync();

                var result =
                    await _botRepository.FeedBackRootExport(DateTime.UtcNow.AddDays(-1 * Constants.FeedBackDelayDays));

                return this.Ok(result);
            }
            else
            {
                return this.Unauthorized();
            }
        }
    }
}