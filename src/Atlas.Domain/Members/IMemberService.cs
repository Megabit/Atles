﻿using System.Threading.Tasks;
using Atlas.Domain.Members.Commands;

namespace Atlas.Domain.Members
{
    public interface IMemberService
    {
        Task CreateAsync(CreateMember command);
        Task ConfirmAsync(ConfirmMember command);
        Task UpdateAsync(UpdateMember command);
        Task SuspendAsync(SuspendMember command);
        Task ReinstateAsync(ReinstateMember command);
        Task<string> DeleteAsync(DeleteMember command);
        Task<string> GenerateDisplayNameAsync();
    }
}
