using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestLS.Auth.Models;
using RestLS.Data.Dtos.Recomendation;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;


[ApiController]
[Route("api/therapies/{therapyId}/appointments/{appointmentId}/recomendations")]
public class RecomendationsController : ControllerBase
{
    private readonly ITherapiesRepository _therapiesRepository;
    private readonly IAppointmentsRepository _appointmentRepository;
    private readonly IRecomendationsRepository _recomendationsRepository;
    private readonly IAuthorizationService _authorizationService;

    public RecomendationsController(IAppointmentsRepository appointmentRepository, ITherapiesRepository therapiesRepository, IRecomendationsRepository recomendationsRepository, IAuthorizationService authorizationService)
    {
        _appointmentRepository = appointmentRepository;
        _therapiesRepository = therapiesRepository;
        _recomendationsRepository = recomendationsRepository;
        _authorizationService = authorizationService;
    }
    
    [HttpGet]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<IEnumerable<RecomendationDto>> GetMany(int therapyId, int appointmentId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return new List<RecomendationDto>();
        
        var appointment = await _appointmentRepository.GetAsync(therapy.Id, appointmentId);
        if (appointment == null) return new List<RecomendationDto>();
        
        
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return new List<RecomendationDto>();
        }
        
        var recomendations = await _recomendationsRepository.GetManyAsync(therapy.Id, appointment.ID);
        return recomendations.Select(o => new RecomendationDto(o.ID, o.Description));
    }

    // /api/topics/1/posts/2
    [HttpGet("{recomendationId}", Name = "GetRecomendation")]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<ActionResult<RecomendationDto>> Get(int therapyId, int appointmentId, int recomendationId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var appointment = await _appointmentRepository.GetAsync(therapy.Id, appointmentId);
        if (appointment == null) return NotFound($"Couldn't find an appointment with id of {appointmentId}");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        
        var recomendation = await _recomendationsRepository.GetAsync(therapy.Id, appointment.ID, recomendationId);
        if (recomendation == null) return NotFound();

        return Ok(new RecomendationDto(recomendation.ID, recomendation.Description));
    }

    [HttpPost]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<ActionResult<RecomendationDto>> Create(int therapyId,int appointmentId, CreateRecomendationDto recomendationDto)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var appointment = await _appointmentRepository.GetAsync(therapyId, appointmentId);
        if (appointment == null) return NotFound($"Couldn't find an appointment with id of {appointmentId}");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var recomendation = new Recomendation{Description = recomendationDto.Description};
        recomendation.Appoint = appointment;
        recomendation.RecomendationDate = DateTime.UtcNow;

        await _recomendationsRepository.CreateAsync(recomendation);

        return Created("GetRecomendation", new RecomendationDto(recomendation.ID, recomendation.Description));
    }

    [HttpPut("{recomendationId}")]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<ActionResult<RecomendationDto>> Update(int therapyId, int appointmentId,int recomendationId, UpdateRecomendationDto updaterecomendationDto)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var appointment = await _appointmentRepository.GetAsync(therapyId, appointmentId);
        if (appointment == null) return NotFound($"Couldn't find an appointment with id of {appointmentId}");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var oldRecomendation = await _recomendationsRepository.GetAsync(therapyId, appointmentId, recomendationId);
        if (oldRecomendation == null)
            return NotFound();

        //oldPost.Body = postDto.Body;
        oldRecomendation.Description = updaterecomendationDto.Description;

        await _recomendationsRepository.UpdateAsync(oldRecomendation);

        return Ok(new RecomendationDto(oldRecomendation.ID, oldRecomendation.Description));
    }

    [HttpDelete("{recomendationId}")]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public async Task<ActionResult> Remove(int therapyId, int appointmentId, int recomendationId)
    {
        var therapy = await _therapiesRepository.GetAsync(therapyId);
        if (therapy == null) return NotFound($"Couldn't find a therapy with id of {therapyId}");
        
        var appointment = await _appointmentRepository.GetAsync(therapyId, appointmentId);
        if (appointment == null) return NotFound($"Couldn't find an appointment with id of {appointmentId}");
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, therapy, PolicyNames.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        
        var recomendation = await _recomendationsRepository.GetAsync(therapyId, appointmentId, recomendationId);
        if (recomendation == null)
            return NotFound();

        await _recomendationsRepository.RemoveAsync(recomendation);

        // 204
        return NoContent();
    }
}