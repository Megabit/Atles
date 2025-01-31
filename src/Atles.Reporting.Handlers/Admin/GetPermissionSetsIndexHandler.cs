﻿using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Domain.Models.PermissionSets;
using Atles.Infrastructure.Queries;
using Atles.Reporting.Models.Admin.PermissionSets.Queries;
using Microsoft.EntityFrameworkCore;
using IndexPageModel = Atles.Reporting.Models.Admin.PermissionSets.IndexPageModel;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetPermissionSetsIndexHandler : IQueryHandler<GetPermissionSetsIndex, IndexPageModel>
    {
        private readonly AtlesDbContext _dbContext;

        public GetPermissionSetsIndexHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IndexPageModel> Handle(GetPermissionSetsIndex query)
        {
            var result = new IndexPageModel();

            var permissionSets = await _dbContext.PermissionSets
                .Include(x => x.Categories)
                .Include(x => x.Forums)
                .Where(x => x.SiteId == query.SiteId && x.Status != PermissionSetStatusType.Deleted)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new IndexPageModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name,
                    IsInUse = permissionSet.Categories.Any() || permissionSet.Forums.Any()
                });
            }

            return result;
        }
    }
}
