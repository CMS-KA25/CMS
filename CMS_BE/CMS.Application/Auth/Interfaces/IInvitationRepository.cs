using CMS.Domain.Auth.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Application.Auth.Interfaces
{
    public interface IInvitationRepository
    {
        Task<Invitation> CreateAsync(Invitation invitation);
        Task<Invitation?> GetByTokenAsync(string token);
        Task<IEnumerable<Invitation>> CreateBulkAsync(IEnumerable<Invitation> invitations);
        Task<Invitation> UpdateAsync(Invitation invitation);
        Task<Invitation?> GetLatestByEmailAsync(string email);
    }
}
