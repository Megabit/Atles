﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atles.Data;
using Atles.Data.Caching;
using Atles.Domain.Handlers.Categories.Commands;
using Atles.Domain.Models.Categories;
using Atles.Domain.Models.Categories.Commands;
using Atles.Domain.Models.PermissionSets;
using Atles.Domain.Models.PermissionSets.Commands;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Handlers.Tests.Categories.Commands
{
    [TestFixture]
    public class CreateCategoryHandlerTests : TestFixtureBase
    {
        [Test]
        public async Task Should_create_new_category_and_add_event()
        {
            using (var dbContext = new AtlesDbContext(Shared.CreateContextOptions()))
            {
                var command = Fixture.Create<CreateCategory>();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<CreateCategory>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var updateValidator = new Mock<IValidator<UpdateCategory>>();

                var sut = new CreateCategoryHandler(dbContext, validator.Object, cacheManager.Object);

                await sut.Handle(command);

                var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.NotNull(category);
                Assert.AreEqual(1, category.SortOrder);
                Assert.NotNull(@event);
            }
        }

        [Test]
        public async Task Should_update_category_and_add_event()
        {
            var options = Shared.CreateContextOptions();
            var permissionSet = new PermissionSet(Guid.NewGuid(), Guid.NewGuid(), "Default", new List<PermissionCommand>());
            var category = new Category(Guid.NewGuid(), permissionSet.SiteId, "Category", 1, permissionSet.Id);

            using (var dbContext = new AtlesDbContext(options))
            {
                dbContext.PermissionSets.Add(permissionSet);
                dbContext.Categories.Add(category);
                await dbContext.SaveChangesAsync();
            }

            using (var dbContext = new AtlesDbContext(options))
            {
                var command = Fixture.Build<UpdateCategory>()
                        .With(x => x.Id, category.Id)
                        .With(x => x.SiteId, category.SiteId)
                    .Create();

                var cacheManager = new Mock<ICacheManager>();

                var validator = new Mock<IValidator<UpdateCategory>>();
                validator
                    .Setup(x => x.ValidateAsync(command, new CancellationToken()))
                    .ReturnsAsync(new ValidationResult());

                var sut = new UpdateCategoryHandler(dbContext, validator.Object, cacheManager.Object);

                await sut.Handle(command);

                var updatedCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == command.Id);
                var @event = await dbContext.Events.FirstOrDefaultAsync(x => x.TargetId == command.Id);

                validator.Verify(x => x.ValidateAsync(command, new CancellationToken()));
                Assert.AreEqual(command.Name, updatedCategory.Name);
                Assert.NotNull(@event);
            }
        }
    }
}
