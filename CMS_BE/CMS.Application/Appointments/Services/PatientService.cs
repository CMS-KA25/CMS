using CMS.Application;
using CMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Application
{
    public class PatientService
    {
        private readonly IPatient _patientrepo;
        public PatientService(IPatient patientrepo)
        {
            _patientrepo = patientrepo;
        }

        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            return await _patientrepo.GetPatients();
        }

        public async Task<Patient> CreatePatient(Patient patient)
        {
            return await _patientrepo.AddPatient(patient);
        }
    }
}
