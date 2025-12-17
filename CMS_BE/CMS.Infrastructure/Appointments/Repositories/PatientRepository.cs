using CMS.Application;
using CMS.Data;
using CMS.Domain;
using Microsoft.EntityFrameworkCore;

namespace CMS.Infrastructure
{
    public class PatientRepository : IPatient
    {
        private readonly CmsDbContext _context;
        public PatientRepository(CmsDbContext context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<Patient>> GetPatients()
        {
            return await _context.Patients.ToListAsync();
        }
        public async Task<Patient> AddPatient(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await  _context.SaveChangesAsync();
            return patient;
        }

       
    }
}
