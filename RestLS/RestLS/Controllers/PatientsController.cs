using Microsoft.AspNetCore.Mvc;
using RestLS.Data.Dtos.Patients;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientsRepository _patientsRepository;
    private readonly IDoctorsRepository _doctorsRepository;

    public PatientsController(IPatientsRepository patientsRepository, IDoctorsRepository doctorsRepository)
    {
        _patientsRepository = patientsRepository;
        _doctorsRepository = doctorsRepository;
    }
    
    // AutoMapper library for making remapping for sending to API
    [HttpGet(Name = "GetPatients")]
    public async Task<IEnumerable<PatientDto>> GetMany()
    {
        var patients = await _patientsRepository.GetManyAsync();
        
        return patients.Select(o => new PatientDto(o.Id, o.Name, o.Lastname, o.PhoneNumb));
    }
    
    
    
    // api/doctors/{doctorID}
    [HttpGet("{patientId}", Name = "GetPatient")]
    public async Task<ActionResult<PatientDto>> Get(int patientId)
    {
        var patient = await _patientsRepository.GetAsync(patientId);
        if (patient == null) return NotFound();

        return Ok(new PatientDto(patient.Id, patient.Name, patient.Lastname, patient.PhoneNumb));
    }
    
    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create(CreatePatientDto createPatientDto)
    {
        var patient = new Patient{Name = createPatientDto.Name, Lastname = createPatientDto.Lastname,
            PhoneNumb = createPatientDto.Phone};
        
        var existingPatient = await _patientsRepository.GetAsync(patient.PhoneNumb);
        if (existingPatient != null)
        {
            return Conflict("Patient with the same phone number already exists.");
        }
        
        var doctor = await _doctorsRepository.GetAsync(patient.PhoneNumb);
        if (doctor != null)
        {
            return Conflict("Someone with the same phone number already exists.");
        }
        
        await _patientsRepository.CreateAsync(patient);
        
        //201
        return Created("", new PatientDto(patient.Id, patient.Name, patient.Lastname, patient.PhoneNumb));
        //return CreatedAtAction("GetDoctor", new { doctorId = doctor.Id }, new DoctorDto(doctor.Name, doctor.Lastname, doctor.Description));
    }
    
    [HttpPut]
    [Route("{patientId}")]
    public async Task<ActionResult<PatientDto>> Update(int patientId, UpdatePatientDto updatePatientDto)
    {
        var patient = await _patientsRepository.GetAsync(patientId);

        if (patient == null)
        {
            return NotFound();
        }

        patient.Name = updatePatientDto.Name;
        patient.Lastname = updatePatientDto.Lastname;
        var doctor = await _doctorsRepository.GetAsync(updatePatientDto.Phone);
        if (doctor != null)
        {
            return Conflict("Someone with the same phone number already exists.");
        }
        if (!string.Equals(patient.PhoneNumb, updatePatientDto.Phone))
        {
            var existingPatient = await _patientsRepository.GetAsync(updatePatientDto.Phone);
            
            if (existingPatient != null)
            {
                return Conflict("Someone with the same phone number already exists.");
            }
        }
        patient.PhoneNumb = updatePatientDto.Phone;
        
        await _patientsRepository.UpdateAsync(patient);

        return Ok(new PatientDto(patient.Id, patient.Name, patient.Lastname, patient.PhoneNumb));
    }
    
    [HttpDelete("{patientId}", Name = "DeletePatient")]
    public async Task<ActionResult> Remove(int patientId)
    {
        var patient = await _patientsRepository.GetAsync(patientId);

        if (patient == null)
        {
            return NotFound();
        }

        await _patientsRepository.RemoveAsync(patient);
        
        //204
        return NoContent();
    }
}