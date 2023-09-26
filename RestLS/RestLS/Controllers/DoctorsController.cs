using Microsoft.AspNetCore.Mvc;
using RestLS.Data.Dtos.Doctors;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorsRepository _doctorsRepository;

    public DoctorsController(IDoctorsRepository doctorsRepository)
    {
        _doctorsRepository = doctorsRepository;
    }
    
    // AutoMapper library for making remapping for sending to API
    [HttpGet]
    public async Task<IEnumerable<DoctorDto>> GetMany()
    {
        var doctors = await _doctorsRepository.GetManyAsync();
        
        return doctors.Select(o => new DoctorDto(o.Id, o.Name, o.Lastname, o.Description));
    }
    
    // api/doctors/{doctorID}
    [HttpGet]
    [Route("{doctorID}", Name = "GetDoctor")]
    public async Task<ActionResult<DoctorDto>> Get(int doctorId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        
        //404
        if (doctor == null)
        {
            return NotFound();
        }
        
        return new DoctorDto(doctor.Id, doctor.Name, doctor.Lastname, doctor.Description);
    }
    
    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create(CreateDoctorDto createDoctorDto)
    {
        var doctor = new Doctor{Name = createDoctorDto.Name,Lastname = createDoctorDto.LastName, Description = createDoctorDto.Description};

        await _doctorsRepository.CreateAsync(doctor);
        
        //201
        return Created("", new DoctorDto(doctor.Id, doctor.Name, doctor.Lastname, doctor.Description));
        //return CreatedAtAction("GetDoctor", new { doctorId = doctor.Id }, new DoctorDto(doctor.Name, doctor.Lastname, doctor.Description));
    }
    
    [HttpPut]
    [Route("{doctorID}")]
    public async Task<ActionResult<DoctorDto>> Update(int doctorId, UpdateDoctorDto updateDoctorDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);

        if (doctor == null)
        {
            return NotFound();
        }

        doctor.Description = updateDoctorDto.Description;
        doctor.Lastname = updateDoctorDto.LastName;
        
        await _doctorsRepository.UpdateAsync(doctor);

        return Ok(new DoctorDto(doctor.Id, doctor.Name, doctor.Lastname, doctor.Description));
    }
    
    [HttpDelete]
    [Route("{doctorID}")]
    public async Task<ActionResult> Remove(int doctorId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);

        if (doctor == null)
        {
            return NotFound();
        }

        await _doctorsRepository.RemoveAsync(doctor);
        
        //204
        return NoContent();
    }
}