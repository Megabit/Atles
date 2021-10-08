﻿using Atles.Domain.Categories.Commands;
using Atles.Domain.Categories.Rules;
using Atles.Domain.Categories.Validators;
using Atles.Domain.PermissionSets;
using Atles.Infrastructure.Queries;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using OpenCqrs.Queries;

namespace Atles.Domain.Tests.Categories.Validators
{
    [TestFixture]
    public class UpdateCategoryValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<UpdateCategory>().With(x => x.Name, string.Empty).Create();

            var querySender = new Mock<IQuerySender>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateCategoryValidator(querySender.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<UpdateCategory>().With(x => x.Name, new string('*', 51)).Create();

            var querySender = new Mock<IQuerySender>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateCategoryValidator(querySender.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<UpdateCategory>();

            var querySender = new Mock<IQuerySender>();
            querySender.Setup(x => x.Send(It.IsAny<IsCategoryNameUnique>())).ReturnsAsync(false);

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateCategoryValidator(querySender.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_permission_set_is_not_valid()
        {
            var command = Fixture.Create<UpdateCategory>();

            var querySender = new Mock<IQuerySender>();

            var permissionSetRules = new Mock<IPermissionSetRules>();
            permissionSetRules.Setup(x => x.IsValidAsync(command.SiteId, command.PermissionSetId)).ReturnsAsync(false);

            var sut = new UpdateCategoryValidator(querySender.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.PermissionSetId, command);
        }
    }
}
