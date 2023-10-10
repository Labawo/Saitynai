using Microsoft.AspNetCore.Mvc;
using RestLS.Data.Dtos.Appoitments;
using RestLS.Data.Repositories;
using RestLS.Data.Entities;

namespace RestLS.Controllers;

[ApiController]
[Route("api/doctors/{doctorId}/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly IDoctorsRepository _doctorsRepository;
    private readonly IAppointmentsRepository _appointmentRepository;
    private readonly IGroupSessionsRepository _groupSessionsRepository;

    public AppointmentController(IAppointmentsRepository appointmentRepository, IDoctorsRepository doctorsRepository, IGroupSessionsRepository groupSessionsRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorsRepository = doctorsRepository;
        _groupSessionsRepository = groupSessionsRepository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<AppointmentDto>> GetMany(int doctorId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return null;
        
        var appoitments = await _appointmentRepository.GetManyAsync(doctor.Id);
        return appoitments.Select(o => new AppointmentDto(o.ID, o.Time, o.Price, o.Doc.Id));
    }

    // /api/topics/1/posts/2
    [HttpGet("{appoitmentId}", Name = "GetAppointment")]
    public async Task<ActionResult<AppointmentDto>> Get(int doctorId, int appoitmentId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");
        
        var appointment = await _appointmentRepository.GetAsync(doctor.Id, appoitmentId);
        if (appointment == null) return NotFound();

        return Ok(new AppointmentDto(appointment.ID, appointment.Time, appointment.Price, appointment.Doc.Id));
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> Create(int doctorId, CreateAppointmentDto appoitmentDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");

        var appoitment = new Appointment{Price = appoitmentDto.Price};
        appoitment.Doc = doctor;
        appoitment.AppointmentDate = DateTime.Now;
        appoitment.IsAvailable = true;
        appoitment.Time = DateTime.Parse(appoitmentDto.Time);

        var existingAppointment = await _appointmentRepository.GetAsync(doctor.Id, appoitment.Time);
        if (existingAppointment != null)
        {
            return Conflict("Appointment at this time already exists.");
        }
        
        var existingGroupSession = await _groupSessionsRepository.GetAsync(doctor.Id, appoitment.Time);
        if (existingGroupSession != null)
        {
            return Conflict("Group session at this time already exists.");
        }

        await _appointmentRepository.CreateAsync(appoitment);

        return Created("GetAppointment", new AppointmentDto(appoitment.ID, appoitment.Time, appoitment.Price, appoitment.Doc.Id));
    }

    [HttpPut("{appoitmentId}")]
    public async Task<ActionResult<AppointmentDto>> Update(int doctorId, int appoitmentId, UpdateAppointmentDto updateappoitmentDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");

        var oldAppoitment = await _appointmentRepository.GetAsync(doctorId, appoitmentId);
        if (oldAppoitment == null)
            return NotFound();

        //oldPost.Body = postDto.Body;
        oldAppoitment.Price = updateappoitmentDto.Price;
        

        if (oldAppoitment.Time != DateTime.Parse(updateappoitmentDto.Time))
        {
            oldAppoitment.Time = DateTime.Parse(updateappoitmentDto.Time);
            
            var existingAppointment = await _appointmentRepository.GetAsync(doctor.Id, oldAppoitment.Time);
            if (existingAppointment != null)
            {
                return Conflict("Appointment at this time already exists.");
            }
        
            var existingGroupSession = await _groupSessionsRepository.GetAsync(doctor.Id, oldAppoitment.Time);
            if (existingGroupSession != null)
            {
                return Conflict("Group session at this time already exists.");
            }
        }
        
        

        await _appointmentRepository.UpdateAsync(oldAppoitment);

        return Ok(new AppointmentDto(oldAppoitment.ID, oldAppoitment.Time, oldAppoitment.Price, oldAppoitment.Doc.Id));
    }

    [HttpDelete("{appoitmentId}")]
    public async Task<ActionResult> Remove(int doctorId, int appoitmentId)
    {
        var appoitment = await _appointmentRepository.GetAsync(doctorId, appoitmentId);
        if (appoitment == null)
            return NotFound();

        await _appointmentRepository.RemoveAsync(appoitment);

        // 204
        return NoContent();
    }
}