using Microsoft.AspNetCore.Mvc;
using RestLS.Data.Dtos.Recomendation;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;


[ApiController]
[Route("api/doctors/{doctorId}/appointments/{appointmentId}/recomendations")]
public class RecomendationsController : ControllerBase
{
    private readonly IDoctorsRepository _doctorsRepository;
    private readonly IAppointmentsRepository _appointmentRepository;
    private readonly IRecomendationsRepository _recomendationsRepository;

    public RecomendationsController(IAppointmentsRepository appointmentRepository, IDoctorsRepository doctorsRepository, IRecomendationsRepository recomendationsRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorsRepository = doctorsRepository;
        _recomendationsRepository = recomendationsRepository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<RecomendationDto>> GetMany(int doctorId, int appointmentId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return null;
        
        var appointment = await _appointmentRepository.GetAsync(doctor.Id, appointmentId);
        if (appointment == null) return null;
        var recomendations = await _recomendationsRepository.GetManyAsync(doctor.Id, appointment.ID);
        return recomendations.Select(o => new RecomendationDto(o.ID, o.Description));
    }

    // /api/topics/1/posts/2
    [HttpGet("{recomendationId}", Name = "GetRecomendation")]
    public async Task<ActionResult<RecomendationDto>> Get(int doctorId, int appointmentId, int recomendationId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");
        
        var appointment = await _appointmentRepository.GetAsync(doctor.Id, appointmentId);
        if (appointment == null) return NotFound($"Couldn't find an appointment with id of {appointmentId}");
        
        var recomendation = await _recomendationsRepository.GetAsync(doctor.Id, appointment.ID, recomendationId);
        if (recomendation == null) return NotFound();

        return Ok(new RecomendationDto(recomendation.ID, recomendation.Description));
    }

    [HttpPost]
    public async Task<ActionResult<RecomendationDto>> Create(int doctorId,int appointmentId, CreateRecomendationDto recomendationDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");
        
        var appointment = await _appointmentRepository.GetAsync(doctorId, appointmentId);
        if (appointment == null) return NotFound($"Couldn't find an appointment with id of {appointmentId}");

        var recomendation = new Recomendation{Description = recomendationDto.Description};
        recomendation.Appoint = appointment;
        recomendation.RecomendationDate = DateTime.Now;

        await _recomendationsRepository.CreateAsync(recomendation);

        return Created("GetRecomendation", new RecomendationDto(recomendation.ID, recomendation.Description));
    }

    [HttpPut("{recomendationId}")]
    public async Task<ActionResult<RecomendationDto>> Update(int doctorId, int appointmentId,int recomendationId, UpdateRecomendationDto updaterecomendationDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");
        
        var appointment = await _appointmentRepository.GetAsync(doctorId, appointmentId);
        if (appointment == null) return NotFound($"Couldn't find an appointment with id of {appointmentId}");

        var oldRecomendation = await _recomendationsRepository.GetAsync(doctorId, appointmentId, recomendationId);
        if (oldRecomendation == null)
            return NotFound();

        //oldPost.Body = postDto.Body;
        oldRecomendation.Description = updaterecomendationDto.Description;

        await _recomendationsRepository.UpdateAsync(oldRecomendation);

        return Ok(new RecomendationDto(oldRecomendation.ID, oldRecomendation.Description));
    }

    [HttpDelete("{recomendationId}")]
    public async Task<ActionResult> Remove(int doctorId, int appointmentId, int recomendationId)
    {
        var recomendation = await _recomendationsRepository.GetAsync(doctorId, appointmentId, recomendationId);
        if (recomendation == null)
            return NotFound();

        await _recomendationsRepository.RemoveAsync(recomendation);

        // 204
        return NoContent();
    }
}