﻿using Atles.Data;
using Atles.Domain.Forums;
using Atles.Domain.Forums.Rules;
using Atles.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Queries;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.Categories.Rules
{
    public class IsForumValidHandler : IQueryHandler<IsForumValid, bool>
    {
        private readonly AtlesDbContext _dbContext;

        public IsForumValidHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(IsForumValid query)
        {
            var any = await _dbContext.Forums
                .AnyAsync(x => x.Category.SiteId == query.SiteId &&
                               x.Id == query.Id &&
                               x.Status == ForumStatusType.Published);
            return any;
        }
    }
}
