﻿using Atles.Data;
using Atles.Data.Caching;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atles.Domain.Models.Forums;
using Atles.Infrastructure;
using Atles.Infrastructure.Queries;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;

namespace Atles.Reporting.Handlers.Public
{
    public class GetCurrentForumsHandler : IQueryHandler<GetCurrentForums, IList<CurrentForumModel>>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IDispatcher _dispatcher;
        public GetCurrentForumsHandler(AtlesDbContext dbContext, ICacheManager cacheManager, IDispatcher sender)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _dispatcher = sender;
        }

        public async Task<IList<CurrentForumModel>> Handle(GetCurrentForums query)
        {
            var site = await _dispatcher.Get(new GetCurrentSite());

            return await _cacheManager.GetOrSetAsync(CacheKeys.CurrentForums(site.Id), async () =>
            {
                var forums = await _dbContext.Forums
                    .Where(x => x.Category.SiteId == site.Id && x.Status == ForumStatusType.Published)
                    .Select(x => new
                    {
                        x.Id,
                        PermissionSetId = x.PermissionSetId ?? x.Category.PermissionSetId
                    })
                    .ToListAsync();

                return forums.Select(forum => new CurrentForumModel
                {
                    Id = forum.Id,
                    PermissionSetId = forum.PermissionSetId
                }).ToList();
            });
        }
    }
}
