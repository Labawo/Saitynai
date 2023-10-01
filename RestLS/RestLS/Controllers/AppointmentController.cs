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

    public AppointmentController(IAppointmentsRepository appointmentRepository, IDoctorsRepository doctorsRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorsRepository = doctorsRepository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<AppointmentDto>> GetAllAsync(int doctorId)
    {
        var appoitments = await _appointmentRepository.GetManyAsync(doctorId);
        return appoitments.Select(o => new AppointmentDto(o.ID, o.Name, o.DoctorId));
    }

    // /api/topics/1/posts/2
    [HttpGet("{appoitmentId}", Name = "GetAppointment")]
    public async Task<ActionResult<AppointmentDto>> GetAsync(int doctorId, int appointmentId)
    {
        var appointment = await _appointmentRepository.GetAsync(doctorId, appointmentId);
        if (appointment == null) return NotFound();

        return Ok(new AppointmentDto(appointment.ID, appointment.Name, appointment.DoctorId));
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> PostAsync(int doctorId, CreateAppointmentDto appoitmentDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");

        var appoitment = new Appointment{Name = appoitmentDto.Name};
        appoitment.DoctorId = doctorId;

        await _appointmentRepository.CreateAsync(appoitment);

        return Created("GetAppointment", new AppointmentDto(appoitment.ID, appoitment.Name, appoitment.DoctorId));
    }

    [HttpPut("{appoitmentId}")]
    public async Task<ActionResult<AppointmentDto>> PostAsync(int doctorId, int appoitmentId, UpdateAppointmentDto updateappoitmentDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");

        var oldAppoitment = await _appointmentRepository.GetAsync(doctorId, appoitmentId);
        if (oldAppoitment == null)
            return NotFound();

        //oldPost.Body = postDto.Body;
        oldAppoitment.Name = updateappoitmentDto.Name;

        await _appointmentRepository.UpdateAsync(oldAppoitment);

        return Ok(new AppointmentDto(oldAppoitment.ID, oldAppoitment.Name, oldAppoitment.DoctorId));
    }

    [HttpDelete("{appoitmentId}")]
    public async Task<ActionResult> DeleteAsync(int doctorId, int appoitmentId)
    {
        var appoitment = await _appointmentRepository.GetAsync(doctorId, appoitmentId);
        if (appoitment == null)
            return NotFound();

        await _appointmentRepository.RemoveAsync(appoitment);

        // 204
        return NoContent();
    }
}