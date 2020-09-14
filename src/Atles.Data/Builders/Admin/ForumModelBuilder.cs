﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Models.Admin.Forums;
using Atles.Domain.Categories;
using Atles.Domain.Forums;
using Atles.Domain.PermissionSets;

namespace Atlas.Data.Builders.Admin
{
    public class ForumModelBuilder : IForumModelBuilder
    {
        private readonly AtlasDbContext _dbContext;

        public ForumModelBuilder(AtlasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync(Guid siteId, Guid? categoryId = null)
        {
            var categories = await _dbContext.Categories
                .Include(x => x.PermissionSet)
                .Where(x => x.SiteId == siteId && x.Status != CategoryStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            if (!categories.Any())
            {
                throw new ApplicationException("No Categories found.");
            }

            var currentCategory = categoryId == null
                ? categories.FirstOrDefault()
                : categories.FirstOrDefault(x => x.Id == categoryId);

            if (currentCategory == null)
            {
                throw new ApplicationException("Category not found.");
            }

            var forums = await _dbContext.Forums
                .Include(x => x.PermissionSet)
                .Where(x => x.CategoryId == currentCategory.Id && x.Status != ForumStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var result = new IndexPageModel();

            foreach (var category in categories)
            {
                result.Categories.Add(new IndexPageModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            foreach (var forum in forums)
            {
                var permissionSetName = !forum.HasPermissionSet()
                    ? $"{currentCategory.PermissionSetName()} (from Category)"
                    : forum.PermissionSetName();

                result.Forums.Add(new IndexPageModel.ForumModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    Slug = forum.Slug,
                    SortOrder = forum.SortOrder,
                    TotalTopics = forum.TopicsCount,
                    TotalReplies = forum.RepliesCount,
                    PermissionSetName = permissionSetName
                });
            }

            return result;
        }

        public async Task<FormComponentModel> BuildCreateFormModelAsync(Guid siteId, Guid? categoryId = null)
        {
            var result = new FormComponentModel();

            var categories = await _dbContext.Categories
                .Where(x => x.SiteId == siteId && x.Status != CategoryStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            if (!categories.Any())
            {
                throw new ApplicationException("No Categories found.");
            }

            foreach (var category in categories)
            {
                result.Categories.Add(new FormComponentModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            var selectedCategoryId = categoryId ?? categories.FirstOrDefault()?.Id ?? Guid.Empty;

            result.Forum = new FormComponentModel.ForumModel
            {
                CategoryId = selectedCategoryId
            };

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == siteId && x.Status == PermissionSetStatusType.Published)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new FormComponentModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }

        public async Task<FormComponentModel> BuildEditFormModelAsync(Guid siteId, Guid id)
        {
            var forum = await _dbContext.Forums
                .FirstOrDefaultAsync(x =>
                    x.Category.SiteId == siteId &&
                    x.Id == id &&
                    x.Status != ForumStatusType.Deleted);

            if (forum == null)
            {
                return null;
            }

            var result = new FormComponentModel
            {
                Forum = new FormComponentModel.ForumModel
                {
                    Id = forum.Id,
                    CategoryId = forum.CategoryId,
                    Name = forum.Name,
                    Slug = forum.Slug,
                    Description = forum.Description,
                    PermissionSetId = forum.PermissionSetId ?? Guid.Empty
                }
            };

            var categories = await _dbContext.Categories
                .Where(x => x.SiteId == siteId && x.Status != CategoryStatusType.Deleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            foreach (var category in categories)
            {
                result.Categories.Add(new FormComponentModel.CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            var permissionSets = await _dbContext.PermissionSets
                .Where(x => x.SiteId == siteId && x.Status == PermissionSetStatusType.Published)
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                result.PermissionSets.Add(new FormComponentModel.PermissionSetModel
                {
                    Id = permissionSet.Id,
                    Name = permissionSet.Name
                });
            }

            return result;
        }
    }
}
