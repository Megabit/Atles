﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Atles.Domain.Posts;

namespace Atlas.Data.Rules
{
    public class TopicRules : ITopicRules
    {
        private readonly AtlasDbContext _dbContext;

        public TopicRules(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsValidAsync(Guid siteId, Guid forumId, Guid id)
        {
            var any = await _dbContext.Posts
                .AnyAsync(x => x.ForumId == forumId &&
                               x.Forum.Category.SiteId == siteId &&
                               x.Id == id &&
                               x.TopicId == null &&
                               x.Status == PostStatusType.Published);
            return any;
        }
    }
}
