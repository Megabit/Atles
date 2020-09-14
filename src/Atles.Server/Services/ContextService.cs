﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Builders;
using Atles.Data.Caching;
using Atles.Domain.Forums;
using Atles.Domain.Users;
using Atles.Models.Public;
using Markdig;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server.Services
{
    public class ContextService : IContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheManager _cacheManager;
        private readonly AtlesDbContext _dbContext;
        private readonly IGravatarService _gravatarService;

        public ContextService(IHttpContextAccessor httpContextAccessor, 
            ICacheManager cacheManager,
            AtlesDbContext dbContext,
            IGravatarService gravatarService)
        {
            _httpContextAccessor = httpContextAccessor;
            _cacheManager = cacheManager;
            _dbContext = dbContext;
            _gravatarService = gravatarService;
        }

        public async Task<CurrentSiteModel> CurrentSiteAsync()
        {
            var currentSite = await _cacheManager.GetOrSetAsync(CacheKeys.CurrentSite("Default"), () => 
                _dbContext.Sites.FirstOrDefaultAsync(x => x.Name == "Default"));

            return new CurrentSiteModel
            {
                Id = currentSite.Id,
                Name = currentSite.Name,
                Title = currentSite.Title,
                Theme = currentSite.PublicTheme,
                CssPublic = currentSite.PublicCss,
                CssAdmin = currentSite.AdminCss,
                Language = currentSite.Language,
                Privacy = Markdown.ToHtml(currentSite.Privacy),
                Terms = Markdown.ToHtml(currentSite.Terms),
                HeadScript = currentSite.HeadScript
            };
        }

        public async Task<CurrentUserModel> CurrentUserAsync()
        {
            var result = new CurrentUserModel();

            var claimsPrincipal = _httpContextAccessor.HttpContext.User;

            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                var identityUserId = _httpContextAccessor.HttpContext.User.Identities.First().Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(identityUserId))
                {
                    var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId);

                    if (user != null)
                    {
                        result = new CurrentUserModel
                        {
                            Id = user.Id,
                            IdentityUserId = user.IdentityUserId,
                            Email = user.Email,
                            DisplayName = user.DisplayName,
                            GravatarHash = _gravatarService.HashEmailForGravatar(user.Email),
                            IsSuspended = user.Status == UserStatusType.Suspended,
                            IsAuthenticated = true
                        };
                    }
                }
            }

            return result;
        }

        public async Task<IList<CurrentForumModel>> CurrentForumsAsync()
        {
            var site = await CurrentSiteAsync();

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