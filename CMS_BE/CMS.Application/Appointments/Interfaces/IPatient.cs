using CMS.Domain;

namespace CMS.Application
{
    public interface IPatient
    {
        Task<IEnumerable<Patient>> GetPatients();
        //Task<Patient> GetPatientById(Guid id);
        Task<Patient> AddPatient(Patient patient);
        //Task UpdatePatient(Patient patient);
        //Task DeletePatient(Guid id);
    }
}
