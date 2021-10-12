﻿using Atles.Data;
using Atles.Domain.Forums;
using Atles.Models.Public.Forums;
using Atles.Reporting.Public.Queries;
using Microsoft.EntityFrameworkCore;
using OpenCqrs;
using OpenCqrs.Queries;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Public
{
    public class GetForumPageHandler : IQueryHandler<GetForumPage, ForumPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ISender _sender;

        public GetForumPageHandler(AtlesDbContext dbContext, ISender sender)
        {
            _dbContext = dbContext;
            _sender = sender;
        }

        public async Task<ForumPageModel> Handle(GetForumPage query)
        {
            var forum = await _dbContext.Forums
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.Slug == query.Slug &&
                    x.Category.SiteId == query.SiteId &&
                    x.Status == ForumStatusType.Published);

            if (forum == null)
            {
                return null;
            }

            var result = new ForumPageModel
            {
                Forum = new ForumPageModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    Description = forum.Description,
                    Slug = forum.Slug
                },
                Topics = await _sender.Send(new GetForumPageTopics { ForumId = forum.Id, Options = query.Options })
            };

            return result;
        }
    }
}