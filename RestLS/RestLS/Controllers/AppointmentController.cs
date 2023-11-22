using System.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        
        var appoitments = await _appointmentRepository.GetManyAsync(therapy.Id);
        return appoitments.Select(o => new AppointmentDto(o.ID, o.Time, o.Price, o.Therapy.Id));
    }

    // /api/topics/1/posts/2
    [HttpGet("{appoitmentId}", Name = "GetAppointment")]
    public async Task<ActionResult<AppointmentDto>> Get(int therapyId, int appoitmentId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var appointment = await _appointmentRepository.GetAsync(therapy.Id, appoitmentId);
        if (appointment == null) return NotFound();

        return Ok(new AppointmentDto(appointment.ID, appointment.Time, appointment.Price, appointment.Therapy.Id));
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
        appoitment.AppointmentDate = DateTime.Now;
        appoitment.IsAvailable = true;
        appoitment.Time = DateTime.Parse(appoitmentDto.Time);

        var existingAppointment = await _appointmentRepository.GetAsync(therapy.Id, appoitment.Time);
        if (existingAppointment != null)
        {
            return Conflict("Appointment at this time already exists.");
        }

        await _appointmentRepository.CreateAsync(appoitment);

        return Created("GetAppointment", new AppointmentDto(appoitment.ID, appoitment.Time, appoitment.Price, appoitment.Therapy.Id));
    }

    [HttpPut("{appoitmentId}")]
    [Authorize(Roles = ClinicRoles.Doctor + "," + ClinicRoles.Admin)]
    public async Task<ActionResult<AppointmentDto>> Update(int therapyId, int appoitmentId, UpdateAppointmentDto updateappoitmentDto)
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

        var oldAppoitment = await _appointmentRepository.GetAsync(therapyId, appoitmentId);
        if (oldAppoitment == null)
            return NotFound();

        //oldPost.Body = postDto.Body;
        oldAppoitment.Price = updateappoitmentDto.Price;
        

        if (oldAppoitment.Time != DateTime.Parse(updateappoitmentDto.Time))
        {
            oldAppoitment.Time = DateTime.Parse(updateappoitmentDto.Time);
            
            var existingAppointment = await _appointmentRepository.GetAsync(therapy.Id, oldAppoitment.Time);
            if (existingAppointment != null)
            {
                return Conflict("Appointment at this time already exists.");
            }
        }
        
        

        await _appointmentRepository.UpdateAsync(oldAppoitment);

        return Ok(new AppointmentDto(oldAppoitment.ID, oldAppoitment.Time, oldAppoitment.Price, oldAppoitment.Therapy.Id));
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