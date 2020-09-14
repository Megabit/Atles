﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Models.Public.Topics
{
    public class TopicPageModel
    {
        public ForumModel Forum { get; set; } = new ForumModel();
        public TopicModel Topic { get; set; } = new TopicModel();
        public ReplyModel Answer { get; set; } = new ReplyModel();
        public PaginatedData<ReplyModel> Replies { get; set; } = new PaginatedData<ReplyModel>();
        public PostModel Post { get; set; } = new PostModel();

        public bool CanReply { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanModerate { get; set; }

        public class ForumModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
        }

        public class TopicModel
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Slug { get; set; }
            public string Content { get; set; }
            public Guid UserId { get; set; }
            public string UserDisplayName { get; set; }
            public DateTime TimeStamp { get; set; }
            public string IdentityUserId { get; set; }
            public string GravatarHash { get; set; }
            public bool Pinned { get; set; }
            public bool Locked { get; set; }
            public bool HasAnswer { get; set; }
        }

        public class ReplyModel
        {
            public Guid Id { get; set; }
            public string Content { get; set; }
            public string OriginalContent { get; set; }
            public string IdentityUserId { get; set; }
            public Guid UserId { get; set; }
            public string UserDisplayName { get; set; }
            public DateTime TimeStamp { get; set; }
            public string GravatarHash { get; set; }
            public bool IsAnswer { get; set; }
        }

        public class PostModel
        {
            public Guid? Id { get; set; }

            [Required]
            public string Content { get; set; }

            public Guid UserId { get; set; }
        }
    }
}
