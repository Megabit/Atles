﻿using System;
using Docs.Attributes;

namespace Atlas.Domain.Forums.Commands
{
    /// <summary>
    /// Request that updates the forum details.
    /// </summary>
    [DocRequest(typeof(Forum))]
    public class UpdateForum : CommandBase
    {
        /// <summary>
        /// The unique identifier of the forum to update.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The new forum category for the forum.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// The new name of the forum.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The new slug of the forum.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The new description of the forum.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The unique identifier of the new permission set for the forum.
        /// </summary>
        public Guid? PermissionSetId { get; set; }
    }
}
