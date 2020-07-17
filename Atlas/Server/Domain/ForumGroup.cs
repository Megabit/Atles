﻿using System;
using System.Collections.Generic;

namespace Atlas.Server.Domain
{
    public class ForumGroup
    {
        public Guid Id { get; private set; }
        public Guid SiteId { get; private set; }
        public string Name { get; private set; }
        public int SortOrder { get; private set; }
        public int TopicsCount { get; private set; }
        public int RepliesCount { get; private set; }
        public StatusType Status { get; private set; }
        public Guid? PermissionSetId { get; private set; }

        public virtual Site Site { get; set; }
        public virtual PermissionSet PermissionSet { get; set; }

        public virtual ICollection<Forum> Forums { get; set; }

        public ForumGroup()
        {

        }

        public ForumGroup(Guid siteId, string name, int sortOrder, Guid? permissionSetId = null)
        {
            Id = Guid.NewGuid();
            SiteId = siteId;
            Name = name;
            SortOrder = sortOrder;
            PermissionSetId = permissionSetId;
            Status = StatusType.Published;
        }
        public void UpdateDetails(string name, Guid? permissionSetId = null)
        {
            Name = name;
            PermissionSetId = permissionSetId;
        }

        public void Delete()
        {
            Status = StatusType.Deleted;
        }

        public string PermissionSetName() => PermissionSet?.Name;

        public bool HasPermissionSet() => PermissionSet != null;
    }    
}
