using CMS.Domain.Clinic.Entities;

namespace CMS.Application.Clinic.Interfaces
{
    public interface ILeaveRepository
    {
        Task<List<Leave>> GetApprovedLeavesForDateAsync(Guid doctorId, DateTime date);
    }
}