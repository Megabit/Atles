﻿using System;
using System.Threading.Tasks;
using Atles.Domain.Models.PermissionSets;
using Atles.Infrastructure;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;
using Atles.Reporting.Models.Shared;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atles.Server.Controllers.Public
{
    [Route("api/public/forums")]
    public class ForumsController : SiteControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly ILogger<ForumsController> _logger;
        private readonly IDispatcher _dispatcher;

        public ForumsController(ISecurityService securityService,
            ILogger<ForumsController> logger,
            IDispatcher dispatcher) : base(dispatcher)
        {
            _securityService = securityService;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<ForumPageModel>> Forum(string slug, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await CurrentSite();
            var user = await CurrentUser();

            var model = await _dispatcher.Get(new GetForumPage { SiteId = site.Id, Slug = slug, Options = new QueryOptions(page, search) });

            if (model == null)
            {
                _logger.LogWarning("Forum not found.", new
                {
                    SiteId = site.Id,
                    ForumSlug = slug,
                    User = User.Identity.Name
                });

                return NotFound();
            }

            var permissions = await _dispatcher.Get(new GetPermissions { SiteId = site.Id, ForumId = model.Forum.Id });

            var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
            var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);

            if (!canViewForum || !canViewTopics)
            {
                _logger.LogWarning("Unauthorized access to forum.", new
                {
                    SiteId = site.Id,
                    ForumSlug = slug,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            model.CanRead = _securityService.HasPermission(PermissionType.Read, permissions);
            model.CanStart = _securityService.HasPermission(PermissionType.Start, permissions) && !user.IsSuspended;

            return model;
        }

        [HttpGet("{id}/topics")]
        public async Task<ActionResult<PaginatedData<ForumPageModel.TopicModel>>> Topics(Guid id, [FromQuery] int? page = 1, [FromQuery] string search = null)
        {
            var site = await CurrentSite();

            var permissions = await _dispatcher.Get(new GetPermissions { SiteId = site.Id, ForumId = id });

            var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions);
            var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions);

            if (!canViewForum || !canViewTopics)
            {
                _logger.LogWarning("Unauthorized access to forum topics.", new
                {
                    SiteId = site.Id,
                    ForumId = id,
                    User = User.Identity.Name
                });

                return Unauthorized();
            }

            var result = await _dispatcher.Get(new GetForumPageTopics { SiteId = site.Id, ForumId = id, Options = new QueryOptions(page, search) });

            return result;
        }
    }
}
