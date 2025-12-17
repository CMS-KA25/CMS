using CMS.Domain.Auth.Entities;
using System;
using System.Threading.Tasks;

namespace CMS.Application.Auth.Interfaces
{
    public interface IVerificationCodeRepository
    {
        Task<VerificationCode> CreateAsync(VerificationCode code);
        Task<VerificationCode?> GetLatestAsync(Guid userId, string purpose);
        Task<VerificationCode> UpdateAsync(VerificationCode code);
    }
}
