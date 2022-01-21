﻿using System;
using Atles.Domain.Models.Categories;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request that creates a new Forum Category.
    /// </summary>
    [DocRequest(typeof(Category))]
    public class CreateCategory : CommandBase
    {
        /// <summary>
        /// The name for the Forum Category.
        /// This value is mandatory.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier of the Permission Set used os default for all Forums within the Forum Category.
        /// This value is mandatory.
        /// </summary>
        public Guid PermissionSetId { get; set; }
    }
}