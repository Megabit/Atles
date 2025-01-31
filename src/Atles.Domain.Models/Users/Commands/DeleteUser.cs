﻿using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Models.Users.Commands
{
    /// <summary>
    /// Request to set the status of a user to deleted and to remove the identity user from the membership database.
    /// </summary>
    [DocRequest(typeof(User))]
    public class DeleteUser : CommandBase
    {
        public string IdentityUserId { get; set; }
    }
}
