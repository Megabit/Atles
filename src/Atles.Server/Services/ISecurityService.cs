﻿using System.Collections.Generic;
using Atles.Domain.Models.PermissionSets;
using Atles.Reporting.Models.Public;

namespace Atles.Server.Services
{
    public interface ISecurityService
    {
        bool HasPermission(PermissionModel model);
        bool HasPermission(PermissionType type, IList<PermissionModel> models);
    }
}
