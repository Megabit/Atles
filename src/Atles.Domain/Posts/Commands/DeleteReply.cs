﻿using System;
using Docs.Attributes;

namespace Atlas.Domain.Posts.Commands
{
    /// <summary>
    /// Request to delete a reply.
    /// </summary>
    [DocRequest(typeof(Post))]
    public class DeleteReply : CommandBase
    {
        /// <summary>
        /// The unique identifier of the reply to delete.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The unique identifier of the forum.
        /// </summary>
        public Guid ForumId { get; set; }

        /// <summary>
        /// The unique identifier of the topic.
        /// </summary>
        public Guid TopicId { get; set; }
    }
}
