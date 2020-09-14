﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Models.Public.Users
{
    public interface IUserModelBuilder
    {
        Task<UserPageModel> BuildUserPageModelAsync(Guid userId, IList<Guid> forumIds);
        Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid userId);
    }
}