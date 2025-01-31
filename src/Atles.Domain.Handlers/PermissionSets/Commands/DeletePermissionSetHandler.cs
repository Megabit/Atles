﻿using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Models;
using Atles.Domain.Models.PermissionSets;
using Atles.Domain.Models.PermissionSets.Commands;
using Atles.Infrastructure.Commands;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Handlers.PermissionSets.Commands
{
    public class DeletePermissionSetHandler : ICommandHandler<DeletePermissionSet>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public DeletePermissionSetHandler(AtlesDbContext dbContext,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task Handle(DeletePermissionSet command)
        {
            var permissionSet = await _dbContext.PermissionSets
                .Include(x => x.Categories)
                .Include(x => x.Forums)
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != PermissionSetStatusType.Deleted);

            if (permissionSet == null)
            {
                throw new DataException($"Permission set with Id {command.Id} not found.");
            }

            if (permissionSet.Categories.Any() || permissionSet.Forums.Any())
            {
                throw new DataException($"Permission set with Id {command.Id} is in use and cannot be deleted.");
            }

            permissionSet.Delete();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Deleted,
                typeof(PermissionSet),
                command.Id));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.PermissionSet(command.Id));
        }
    }
}
