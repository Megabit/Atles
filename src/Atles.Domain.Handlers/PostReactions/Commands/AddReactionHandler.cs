﻿using Atles.Data;
using Atles.Domain.PostReactions;
using Atles.Domain.PostReactions.Commands;
using Atles.Domain.Posts;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System.Data;
using System.Threading.Tasks;

namespace Atles.Domain.Handlers.PostReactions.Commands
{
    public class AddReactionHandler : ICommandHandler<AddReaction>
    {
        private readonly AtlesDbContext _dbContext;

        public AddReactionHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(AddReaction command)
        {
            var post = await _dbContext.Posts
                .FirstOrDefaultAsync(x =>
                    x.Forum.Category.SiteId == command.SiteId &&
                    x.Id == command.PostId &&
                    x.Status != PostStatusType.Deleted);

            if (post == null)
            {
                throw new DataException($"Post with Id {command.Id} not found.");
            }

            if (command.Like)
            {
                post.IncreaseLikesCount();
            }
            else
            {
                post.IncreaseDislikesCount();
            }

            var postLike = new PostReaction(command.PostId, command.UserId, command.Like);

            _dbContext.PostLikes.Add(postLike);

            await _dbContext.SaveChangesAsync();
        }
    }
}