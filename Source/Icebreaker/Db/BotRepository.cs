//----------------------------------------------------------------------------------------------
// <copyright file="IcebreakerBotDataProvider.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Icebreaker.Db.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Icebreaker.Db
{
    /// <summary>
    /// Data provider routines
    /// </summary>
    public class BotRepository
    {
        // Request the minimum throughput by default
        private const int DefaultRequestThroughput = 400;

        private readonly TelemetryClient telemetryClient;
        private readonly Lazy<Task> initializeTask;
        private DocumentClient documentClient;
        private Database database;
        private DocumentCollection teamsCollection;
        private DocumentCollection usersCollection;
        private DocumentCollection usersMatchInfoCollection;
        private DocumentCollection botLastMessageInfoCollection;
        private DocumentCollection fbRootInfoCollection;
        private DocumentCollection fbDetailInfoCollection;
        private DocumentCollection fbCommentInfoCollection;
        private DocumentCollection unknownMessageInfoCollection;
        private DocumentCollection userDetailsCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotRepository"/> class.
        /// </summary>
        /// <param name="telemetryClient">The telemetry client to use</param>
        public BotRepository(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
            this.initializeTask = new Lazy<Task>(() => this.InitializeAsync());
        }

        /// <summary>
        /// Updates team installation status in store. If the bot is installed, the info is saved, otherwise info for the team is deleted.
        /// </summary>
        /// <param name="team">The team installation info</param>
        /// <param name="installed">Value that indicates if bot is installed</param>
        /// <returns>Tracking task</returns>
        public async Task TeamInstallUpdate(TeamInstallInfo team, bool installed)
        {
            await this.EnsureInitializedAsync();

            if (installed)
            {
                var response = await this.documentClient.UpsertDocumentAsync(this.teamsCollection.SelfLink, team);
            }
            else
            {
                var documentUri = UriFactory.CreateDocumentUri(this.database.Id, this.teamsCollection.Id, team.Id);
                var response = await this.documentClient.DeleteDocumentAsync(documentUri, new RequestOptions { PartitionKey = new PartitionKey(team.Id) });
            }
        }

        /// <summary>
        /// Get the list of teams to which the app was installed.
        /// </summary>
        /// <returns>List of installed teams</returns>
        public async Task<IList<TeamInstallInfo>> InstalledTeamsGet()
        {
            await this.EnsureInitializedAsync();

            var installedTeams = new List<TeamInstallInfo>();

            try
            {
                using (var lookupQuery = this.documentClient
                    .CreateDocumentQuery<TeamInstallInfo>(this.teamsCollection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true })
                    .AsDocumentQuery())
                {
                    while (lookupQuery.HasMoreResults)
                    {
                        var response = await lookupQuery.ExecuteNextAsync<TeamInstallInfo>();
                        installedTeams.AddRange(response);
                    }
                }
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex.InnerException);
            }

            return installedTeams;
        }

        /// <summary>
        /// Returns the team that the bot has been installed to
        /// </summary>
        /// <param name="teamId">The team id</param>
        /// <returns>Team that the bot is installed to</returns>
        public async Task<TeamInstallInfo> InstalledTeamGet(string teamId)
        {
            await this.EnsureInitializedAsync();

            // Get team install info
            try
            {
                var documentUri = UriFactory.CreateDocumentUri(this.database.Id, this.teamsCollection.Id, teamId);
                return await this.documentClient.ReadDocumentAsync<TeamInstallInfo>(documentUri, new RequestOptions { PartitionKey = new PartitionKey(teamId) });
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex.InnerException);
                return null;
            }
        }

        /// <summary>
        /// Get the stored information about the given user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User information</returns>
        public async Task<UserOptInStatusInfo> UserInfoGet(string userId)
        {
            await this.EnsureInitializedAsync();

            try
            {
                var documentUri = UriFactory.CreateDocumentUri(this.database.Id, this.usersCollection.Id, userId);
                return await this.documentClient.ReadDocumentAsync<UserOptInStatusInfo>(documentUri, new RequestOptions { PartitionKey = new PartitionKey(userId) });
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex.InnerException);
                return null;
            }
        }

        /// <summary>
        /// Set the user info for the given user
        /// </summary>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        /// <param name="optedIn">User opt-in status</param>
        /// <param name="serviceUrl">User service URL</param>
        /// <returns>Tracking task</returns>
        public async Task UserInfoSet(string tenantId, string userId, bool optedIn, string serviceUrl)
        {
            await this.EnsureInitializedAsync();

            var userInfo = new UserOptInStatusInfo
            {
                TenantId = tenantId,
                UserId = userId,
                OptedIn = optedIn,
                ServiceUrl = serviceUrl
            };
            await this.documentClient.UpsertDocumentAsync(this.usersCollection.SelfLink, userInfo);
        }


        /// <summary>
        /// Save userMatchInfo
        /// </summary>
        /// <returns>Tracking task</returns>
        public async Task UserMatchInfoSave(string senderEmail, string senderAadId, string receiverEmail, string recipientAadId)
        {
            await this.EnsureInitializedAsync();

            var userInfo = new UserMatchInfo()
            {
                SenderEmail = senderEmail,
                SenderAadId = senderAadId,
                RecipientEmail = receiverEmail,
                RecipientAadId = recipientAadId,
                Created = DateTime.Now.ToUniversalTime(),
            };
            await this.documentClient.UpsertDocumentAsync(this.usersMatchInfoCollection.SelfLink, userInfo);
        }

        public async Task BotLastMessageUpdate(string userAadId, string email, string message)
        {
            await this.EnsureInitializedAsync();

            var entity = new BotLastMessageInfo
            {
                UserEmail = email,
                UserId = userAadId,
                Message = message,
                Modified = DateTime.UtcNow
            };
            await this.documentClient.UpsertDocumentAsync(this.botLastMessageInfoCollection.SelfLink, entity);
        }

        public async Task<BotLastMessageInfo> BotLastMessageGet(string userAadId)
        {
            await this.EnsureInitializedAsync();


            var documentUri =
                UriFactory.CreateDocumentUri(this.database.Id, this.botLastMessageInfoCollection.Id, userAadId);
            return await this.documentClient.ReadDocumentAsync<BotLastMessageInfo>(documentUri,
                new RequestOptions {PartitionKey = new PartitionKey(userAadId) });
        }

        public async Task<List<UserMatchInfo>> UserMatchInfoSearchByDate(DateTime time)
        {
            await this.EnsureInitializedAsync();

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            IQueryable<UserMatchInfo> queryable = documentClient
                .CreateDocumentQuery<UserMatchInfo>(this.usersMatchInfoCollection.SelfLink, option)
                .Where(p => p.Created > time);

            var result = queryable.ToList();
            return result;
        }

        public async Task<UserMatchInfo> UserMatchInfoSearchByDateAndUser(DateTime time, string userAadId)
        {
            await this.EnsureInitializedAsync();

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            IQueryable<UserMatchInfo> queryable = documentClient
                .CreateDocumentQuery<UserMatchInfo>(this.usersMatchInfoCollection.SelfLink, option)
                .OrderByDescending(p => p.Created)
                .Where(p => p.Created > time && p.SenderAadId == userAadId);

            var result = queryable.ToList().FirstOrDefault();
            return result;
        }

        public async Task FeedbackRootCreate(string userEmail, string userAadId, string companionUserEmail, string companionUserAadId, string type)
        {
            await this.EnsureInitializedAsync();

            var entity = new FeedBackRootInfo
            {
                UserEmail = userEmail,
                UserAadId = userAadId,
                CompanionUserEmail = companionUserEmail,
                CompanionUserAadId = companionUserAadId,
                Type = type,
                Created = DateTime.UtcNow
            };
            await this.documentClient.UpsertDocumentAsync(this.fbRootInfoCollection.SelfLink, entity);
        }

        public async Task FeedbackDetailCreate(string userEmail, string userAadId, string companionUserEmail, string companionUserAadId, string type, string rootType)
        {
            await this.EnsureInitializedAsync();

            var entity = new FeedBackDetailInfo
            {
                UserEmail = userEmail,
                UserAadId = userAadId,
                CompanionUserEmail = companionUserEmail,
                CompanionUserAadId = companionUserAadId,
                Type = type,
                RootType = rootType,
                Created = DateTime.UtcNow
            };
            await this.documentClient.UpsertDocumentAsync(this.fbDetailInfoCollection.SelfLink, entity);
        }

        public async Task FeedbackCommentCreate(string userEmail, string userAadId, string companionUserEmail, string companionUserAadId, string detailType, string rootType, string comment)
        {
            await this.EnsureInitializedAsync();

            var entity = new FeedBackCommentInfo
            {
                UserEmail = userEmail,
                UserAadId = userAadId,
                CompanionUserEmail = companionUserEmail,
                CompanionUserAadId = companionUserAadId,
                DetailType = detailType,
                RootType = rootType,
                Comment = comment,
                Created = DateTime.UtcNow
            };
            await this.documentClient.UpsertDocumentAsync(this.fbCommentInfoCollection.SelfLink, entity);
        }

        public async Task UnknownMessageCreate(string userEmail, string userAadId, string message)
        {
            await this.EnsureInitializedAsync();

            var entity = new UnknownMessageInfo
            {
                UserEmail = userEmail,
                UserAadId = userAadId,
                Message = message,
                Created = DateTime.UtcNow
            };
            await this.documentClient.UpsertDocumentAsync(this.unknownMessageInfoCollection.SelfLink, entity);
        }

        public async Task UserDetailsUpdate(string aaId, string givenName, string name, string email, string tennantId)
        {
            await this.EnsureInitializedAsync();

            var entity = new UserDetailsInfo
            {
                UserId = aaId,
                GivenName = givenName,
                Name = name,
                Email = email,
                TenantId = tennantId
            };
            await this.documentClient.UpsertDocumentAsync(this.userDetailsCollection.SelfLink, entity);
        }

        public async Task<UserDetailsInfo> UserDetailsGet(string aaId)
        {
            await this.EnsureInitializedAsync();

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            IQueryable<UserDetailsInfo> queryable = documentClient
                .CreateDocumentQuery<UserDetailsInfo>(this.userDetailsCollection.SelfLink, option)
                .Where(p => p.Id == aaId);

            var result = queryable.ToList().FirstOrDefault();
            return result;
        }

        public async Task<List<FeedBackRootInfo>> FeedBackRootExport(DateTime dateTime)
        {
            await this.EnsureInitializedAsync();

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            IQueryable<FeedBackRootInfo> queryable = documentClient
                .CreateDocumentQuery<FeedBackRootInfo>(this.usersMatchInfoCollection.SelfLink, option)
                .OrderBy(p => p.Created)
                .Where(p => p.Created > dateTime);

            var result = queryable.ToList();
            return result;
        }

        /// <summary>
        /// Initializes the database connection.
        /// </summary>
        /// <returns>Tracking task</returns>
        private async Task InitializeAsync()
        {
            this.telemetryClient.TrackTrace("Initializing data store");

            var endpointUrl = CloudConfigurationManager.GetSetting("CosmosDBEndpointUrl");
            var primaryKey = CloudConfigurationManager.GetSetting("CosmosDBKey");
            var databaseName = CloudConfigurationManager.GetSetting("CosmosDBDatabaseName");
            var teamsCollectionName = CloudConfigurationManager.GetSetting("CosmosCollectionTeams");
            var usersCollectionName = CloudConfigurationManager.GetSetting("CosmosCollectionUsers");
            var usersMatchInfoCollectionName = "UsersMatchInfo";
            var botLastMessageCollectionName = "BotLastMessage";
            var fbRootInfoCollectionName = "FeedbackRoot";
            var fbDetailInfoCollectionName = "FeedbackDetail";
            var fbCommentInfoCollectionName = "FeedbackComment";
            var unknownMessageInfoCollectionName = "UnknownMessage";
            var userDetailsCollectionName = "UserDetails";

            this.documentClient = new DocumentClient(new Uri(endpointUrl), primaryKey);

            var requestOptions = new RequestOptions { OfferThroughput = DefaultRequestThroughput };
            bool useSharedOffer = true;

            // Create the database if needed
            try
            {
                this.database = await this.documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName }, requestOptions);
            }
            catch (DocumentClientException ex)
            {
                if (ex.Error?.Message?.Contains("SharedOffer is Disabled") ?? false)
                {
                    this.telemetryClient.TrackTrace("Database shared offer is disabled for the account, will provision throughput at container level", SeverityLevel.Information);
                    useSharedOffer = false;

                    this.database = await this.documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName });
                }
                else
                {
                    throw;
                }
            }

            teamsCollection = await EnsureDocumentCollection(teamsCollectionName, useSharedOffer, requestOptions);
            usersCollection = await EnsureDocumentCollection(usersCollectionName, useSharedOffer, requestOptions);
            usersMatchInfoCollection = await EnsureDocumentCollection(usersMatchInfoCollectionName, useSharedOffer,requestOptions);
            botLastMessageInfoCollection = await EnsureDocumentCollection(botLastMessageCollectionName, useSharedOffer, requestOptions);
            fbRootInfoCollection = await EnsureDocumentCollection(fbRootInfoCollectionName, useSharedOffer, requestOptions);
            fbDetailInfoCollection = await EnsureDocumentCollection(fbDetailInfoCollectionName, useSharedOffer, requestOptions);
            fbCommentInfoCollection = await EnsureDocumentCollection(fbCommentInfoCollectionName, useSharedOffer, requestOptions);
            unknownMessageInfoCollection = await EnsureDocumentCollection(unknownMessageInfoCollectionName, useSharedOffer, requestOptions);
            userDetailsCollection = await EnsureDocumentCollection(userDetailsCollectionName, useSharedOffer, requestOptions);

            this.telemetryClient.TrackTrace("Data store initialized");
        }

        /// <summary>
        /// Get or create collection
        /// </summary>
        private async Task<DocumentCollection> EnsureDocumentCollection(string collectionName, bool useSharedOffer, RequestOptions requestOptions)
        {
            // Get a reference to the Teams collection, creating it if needed
            var collectionDefinition = new DocumentCollection
            {
                Id = collectionName,
            };
            collectionDefinition.PartitionKey.Paths.Add("/id");

            var collection = await this.documentClient.CreateDocumentCollectionIfNotExistsAsync(this.database.SelfLink, collectionDefinition, useSharedOffer ? null : requestOptions);
            return collection;
        }

        private async Task EnsureInitializedAsync()
        {
            await this.initializeTask.Value;
        }
    }
}