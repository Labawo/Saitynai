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

    public PatientsController(IPatientsRepository patientsRepository)
    {
        _patientsRepository = patientsRepository;
    }
    
    // AutoMapper library for making remapping for sending to API
    [HttpGet(Name = "GetPatients")]
    public async Task<IEnumerable<PatientDto>> GetMany()
    {
        var patients = await _patientsRepository.GetManyAsync();
        
        return patients.Select(o => new PatientDto(o.Id, o.Name, o.Lastname, o.PhoneNumb, o.Email));
    }
    
    
    
    // api/doctors/{doctorID}
    [HttpGet("{patientId}", Name = "GetPatient")]
    public async Task<ActionResult<PatientDto>> Get(int patientId)
    {
        var patient = await _patientsRepository.GetAsync(patientId);
        if (patient == null) return NotFound();

        return Ok(new PatientDto(patient.Id, patient.Name, patient.Lastname, patient.PhoneNumb, patient.Email));
    }
    
    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create(CreatePatientDto createPatientDto)
    {
        var patient = new Patient{Name = createPatientDto.Name, Lastname = createPatientDto.Lastname,
            PhoneNumb = createPatientDto.Phone, Email = createPatientDto.Email};
        

        await _patientsRepository.CreateAsync(patient);
        
        //201
        return Created("", new PatientDto(patient.Id, patient.Name, patient.Lastname, patient.PhoneNumb, patient.Email));
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
        
        patient.Lastname = updatePatientDto.Lastname;
        patient.Email = updatePatientDto.Email;
        patient.PhoneNumb = updatePatientDto.Phone;
        
        await _patientsRepository.UpdateAsync(patient);

        return Ok(new PatientDto(patient.Id, patient.Name, patient.Lastname, patient.PhoneNumb, patient.Email));
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