using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using RestLS.Auth.Models;
using RestLS.Data.Dtos.Appoitments;
using RestLS.Data.Repositories;
using RestLS.Data.Entities;

namespace RestLS.Controllers;

[ApiController]
[Route("api/therapies/{therapyId}/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly ITherapiesRepository _therapiesRepository;
    private readonly IAppointmentsRepository _appointmentRepository;
    private readonly IAuthorizationService _authorizationService;

    public AppointmentController(IAppointmentsRepository appointmentRepository, ITherapiesRepository therapiesRepository, IAuthorizationService authorizationService)
    {
        _appointmentRepository = appointmentRepository;
        _therapiesRepository = therapiesRepository;
        _authorizationService = authorizationService;
    }
    
    [HttpGet]
    public async Task<IEnumerable<AppointmentDto>> GetMany(int therapyId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return new List<AppointmentDto>();

        var appointments = await _appointmentRepository.GetManyAsync(therapy.Id);

        if (User.IsInRole(ClinicRoles.Patient))
        {
            // Filter appointments to show only available appointments for patients
            appointments = appointments.Where(appointment => appointment.PatientId == null && appointment.Time > DateTime.UtcNow).ToList();
        }

        return appointments.Select(o => new AppointmentDto(o.ID, o.Time, o.Price, o.PatientId));
    }

    // /api/topics/1/posts/2
    [HttpGet("{appoitmentId}", Name = "GetAppointment")]
    public async Task<ActionResult<AppointmentDto>> Get(int therapyId, int appoitmentId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var appointment = await _appointmentRepository.GetAsync(therapy.Id, appoitmentId);
        if (appointment == null) return NotFound();

        return Ok(new AppointmentDto(appointment.ID, appointment.Time, appointment.Price, appointment.PatientId));
    }

    [HttpPost]
    [Authorize(Roles = ClinicRoles.Doctor + "," + ClinicRoles.Admin)]
    public async Task<ActionResult<AppointmentDto>> Create(int therapyId, CreateAppointmentDto appoitmentDto)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!User.IsInRole(ClinicRoles.Admin))
        {
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
        }
        
        var appoitment = new Appointment{Price = appoitmentDto.Price};
        appoitment.Therapy = therapy;
        appoitment.AppointmentDate = new DateTime();
        appoitment.IsAvailable = true;

        var existingAppointments = await _appointmentRepository.GetManyForDoctorAsync(therapy.DoctorId);
        DateTime oneWeekFromNow = DateTime.UtcNow.AddDays(7);

        var weeklyAppointments = existingAppointments.Where(appointment =>
            appointment.Time >= DateTime.UtcNow && appointment.Time <= oneWeekFromNow
        );
        if (existingAppointments.Any(appointment => appointment.Time >= DateTime.Parse(appoitmentDto.Time) && appointment.Time <= DateTime.Parse(appoitmentDto.Time).AddHours(1)) && weeklyAppointments.Count() >= 12)
        {
            return Conflict("Appointment at this time already exists or you have reached appointment limit.");
        }
            
        appoitment.Time = DateTime.Parse(appoitmentDto.Time);

        await _appointmentRepository.CreateAsync(appoitment);

        return Created("GetAppointment", new AppointmentDto(appoitment.ID, appoitment.Time, appoitment.Price, appoitment.PatientId));
    }

    [HttpPut("{appoitmentId}")]
    [Authorize(Roles = ClinicRoles.Doctor + "," + ClinicRoles.Admin)]
    public async Task<ActionResult<AppointmentDto>> Update(int therapyId, int appoitmentId, UpdateAppointmentDto updateappoitmentDto)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var oldAppoitment = await _appointmentRepository.GetAsync(therapyId, appoitmentId);
        if (oldAppoitment == null)
            return NotFound();

        //oldPost.Body = postDto.Body;
        oldAppoitment.Price = updateappoitmentDto.Price;
        

        if (oldAppoitment.Time != DateTime.Parse(updateappoitmentDto.Time))
        {
            var existingAppointments = await _appointmentRepository.GetManyForDoctorAsync(oldAppoitment.Therapy.DoctorId);
            if (existingAppointments.Any(appointment => appointment.Time >= DateTime.Parse(updateappoitmentDto.Time) && appointment.Time <= DateTime.Parse(updateappoitmentDto.Time).AddHours(1)))
            {
                return Conflict("Appointment at this time already exists.");
            }
            
            oldAppoitment.Time = DateTime.Parse(updateappoitmentDto.Time);
        }
        
        await _appointmentRepository.UpdateAsync(oldAppoitment);

        return Ok(new AppointmentDto(oldAppoitment.ID, oldAppoitment.Time, oldAppoitment.Price, oldAppoitment.PatientId));
    }
    
    [HttpPut("{appoitmentId}/select")]
    [Authorize(Roles = ClinicRoles.Patient)]
    public async Task<ActionResult<AppointmentDto>> Select(int therapyId, int appoitmentId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");

        var oldAppoitment = await _appointmentRepository.GetAsync(therapyId, appoitmentId);
        if (oldAppoitment == null)
            return NotFound();

        string userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var existingAppointments = await _appointmentRepository.GetManyForPatientAsync(userId);
        DateTime oneWeekFromNow = DateTime.UtcNow.AddDays(7);

        var weeklyAppointments = existingAppointments.Where(appointment =>
            appointment.Time >= DateTime.UtcNow && appointment.Time <= oneWeekFromNow
        );
        if (existingAppointments.Any(appointment => appointment.Time >= oldAppoitment.Time && appointment.Time <= oldAppoitment.Time.AddHours(1)) && weeklyAppointments.Count() >= 4)
        {
            return Conflict("Appointment at this time already exists or you have reached appointment limit.");
        }
        
        oldAppoitment.IsAvailable = false;
        oldAppoitment.PatientId = userId;
        
        await _appointmentRepository.UpdateAsync(oldAppoitment);

        return Ok(new AppointmentDto(oldAppoitment.ID, oldAppoitment.Time, oldAppoitment.Price, oldAppoitment.PatientId));
    }

    [HttpDelete("{appoitmentId}")]
    [Authorize(Roles = ClinicRoles.Doctor + "," + ClinicRoles.Admin)]
    public async Task<ActionResult> Remove(int therapyId, int appoitmentId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!User.IsInRole(ClinicRoles.Admin))
        {
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
        }
        
        var appoitment = await _appointmentRepository.GetAsync(therapyId, appoitmentId);
        if (appoitment == null)
            return NotFound();

        await _appointmentRepository.RemoveAsync(appoitment);

        // 204
        return NoContent();
    }
}