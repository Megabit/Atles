﻿using System.Data;
using System.Threading.Tasks;
using Atlas.Data.Caching;
using Atlas.Domain;
using Atlas.Domain.Sites;
using Atlas.Domain.Sites.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Services
{
    public class SiteService : ISiteService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IValidator<UpdateSite> _updateValidator;

        public SiteService(AtlasDbContext dbContext,
            ICacheManager cacheManager,
            IValidator<UpdateSite> updateValidator)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _updateValidator = updateValidator;
        }

        public async Task UpdateAsync(UpdateSite command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var site = await _dbContext.Sites
                .FirstOrDefaultAsync(x => 
                    x.Id == command.SiteId);

            if (site == null)
            {
                throw new DataException($"Site with Id {command.SiteId} not found.");
            }

            site.UpdateDetails(command.Title, 
                command.Theme, 
                command.Css, 
                command.Language, 
                command.Privacy, 
                command.Terms,
                command.HeadScript);

            _dbContext.Events.Add(new Event(site.Id,
                command.UserId,
                EventType.Updated,
                typeof(Site),
                site.Id,
                new
                {
                    site.Title,
                    site.PublicTheme,
                    site.PublicCss,
                    site.Language,
                    site.Privacy,
                    site.Terms,
                    site.HeadScript
                }));

            await _dbContext.SaveChangesAsync();

            _cacheManager.Remove(CacheKeys.CurrentSite(site.Name));
        }
    }
}
