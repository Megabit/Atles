﻿using Atles.Data;
using Atles.Domain.Categories;
using Atles.Domain.Forums;
using Atles.Models.Admin.Categories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Admin.Categories
{
    public class GetCategoriesIndexHandler : IRequestHandler<GetCategoriesIndex, IndexPageModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetCategoriesIndexHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IndexPageModel> Handle(GetCategoriesIndex query, CancellationToken cancellationToken)
        {
            var result = new IndexPageModel();

            var categories = await _dbContext.Categories
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == query.SiteId && x.Status != CategoryStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(cancellationToken: cancellationToken);

            foreach (var category in categories)
            {
                var forumsCount = await _dbContext.Forums
                    .Where(x =>
                        x.CategoryId == category.Id &&
                        x.Status != ForumStatusType.Deleted)
                    .CountAsync(cancellationToken: cancellationToken);

                result.Categories.Add(new IndexPageModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    SortOrder = category.SortOrder,
                    TotalForums = forumsCount,
                    TotalTopics = category.TopicsCount,
                    TotalReplies = category.RepliesCount,
                    PermissionSetName = category.PermissionSetName()
                });
            }

            return result;
        }
    }
}