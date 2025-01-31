﻿using Atles.Data;
using Atles.Reporting.Handlers.Services;
using Markdig;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Atles.Domain.Models.Posts;
using Atles.Infrastructure;
using Atles.Infrastructure.Queries;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;

namespace Atles.Reporting.Handlers.Public
{
    public class GetTopicPageHandler : IQueryHandler<GetTopicPage, TopicPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IDispatcher _dispatcher;
        private readonly IGravatarService _gravatarService;
        public GetTopicPageHandler(AtlesDbContext dbContext, IDispatcher sender, IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _dispatcher = sender;
            _gravatarService = gravatarService;
        }

        public async Task<TopicPageModel> Handle(GetTopicPage query)
        {
            var topic = await _dbContext.Posts
                .Include(x => x.PostReactionCounts)
                .Include(x => x.Forum).ThenInclude(x => x.Category)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x =>
                    x.TopicId == null &&
                    x.Forum.Category.SiteId == query.SiteId &&
                    x.Forum.Slug == query.ForumSlug &&
                    x.Slug == query.TopicSlug &&
                    x.Status == PostStatusType.Published);

            if (topic == null)
            {
                return null;
            }

            var result = new TopicPageModel
            {
                Forum = new TopicPageModel.ForumModel
                {
                    Id = topic.Forum.Id,
                    Name = topic.Forum.Name,
                    Slug = topic.Forum.Slug
                },
                Topic = new TopicPageModel.TopicModel
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    Slug = topic.Slug,
                    Content = Markdown.ToHtml(topic.Content),
                    UserId = topic.CreatedByUser.Id,
                    UserDisplayName = topic.CreatedByUser.DisplayName,
                    TimeStamp = topic.CreatedOn,
                    IdentityUserId = topic.CreatedByUser.IdentityUserId,
                    GravatarHash = _gravatarService.GenerateEmailHash(topic.CreatedByUser.Email),
                    Pinned = topic.Pinned,
                    Locked = topic.Locked,
                    HasAnswer = topic.HasAnswer,
                    Reactions = topic.PostReactionCounts.Select(x => new TopicPageModel.ReactionModel { Type = x.Type, Count = x.Count }).ToList()
                },
                Replies = await _dispatcher.Get(new GetTopicPageReplies { TopicId = topic.Id, Options = query.Options })
            };

            if (topic.HasAnswer)
            {
                var answer = await _dbContext.Posts
                    .Include(x => x.CreatedByUser)
                    .Where(x =>
                        x.TopicId == topic.Id &&
                        x.Status == PostStatusType.Published &&
                        x.IsAnswer)
                    .FirstOrDefaultAsync();

                if (answer != null)
                {
                    result.Answer = new TopicPageModel.ReplyModel
                    {
                        Id = answer.Id,
                        Content = Markdown.ToHtml(answer.Content),
                        OriginalContent = answer.Content,
                        IdentityUserId = answer.CreatedByUser.IdentityUserId,
                        UserId = answer.CreatedByUser.Id,
                        UserDisplayName = answer.CreatedByUser.DisplayName,
                        TimeStamp = answer.CreatedOn,
                        GravatarHash = _gravatarService.GenerateEmailHash(answer.CreatedByUser.Email),
                        IsAnswer = answer.IsAnswer
                    };
                }
            }

            return result;
        }
    }
}
