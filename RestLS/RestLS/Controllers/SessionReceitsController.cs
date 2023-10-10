using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using RestLS.Data.Dtos.SessionReceits;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api/doctors/{doctorId}/groupsessions/{groupSessionId}/receits")]
public class SessionReceitsController : ControllerBase
{
    private readonly IDoctorsRepository _doctorsRepository;
    private readonly IGroupSessionsRepository _groupSessionsRepository;
    private readonly ISessionReceitsRepository _sessionReceitsRepository;

    public SessionReceitsController(IGroupSessionsRepository groupSessionsRepository, IDoctorsRepository doctorsRepository, ISessionReceitsRepository sessionReceitsRepository)
    {
        _groupSessionsRepository = groupSessionsRepository;
        _doctorsRepository = doctorsRepository;
        _sessionReceitsRepository = sessionReceitsRepository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<SessionReceitDto>> GetMany(int doctorId, int groupSessionId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return null;
        
        var groupSession = await _groupSessionsRepository.GetAsync(doctor.Id, groupSessionId);
        if (groupSession == null) return null;
        
        var sessionReceits = await _sessionReceitsRepository.GetManyAsync(doctor.Id, groupSession.Id);
        return sessionReceits.Select(o => new SessionReceitDto(o.Id, o.GroupSes.Id));
    }

    // /api/topics/1/posts/2
    [HttpGet("{sessionReceitId}", Name = "GetReceit")]
    public async Task<ActionResult<SessionReceitDto>> Get(int doctorId, int groupSessionId, int sessionReceitId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");
        
        var groupSession = await _groupSessionsRepository.GetAsync(doctor.Id, groupSessionId);
        if (groupSession == null) return NotFound($"Couldn't find an group session with id of {groupSessionId}");
        
        var sessionReceit = await _sessionReceitsRepository.GetAsync(doctor.Id, groupSession.Id, sessionReceitId);
        if (sessionReceit == null) return NotFound();

        return Ok(new SessionReceitDto(sessionReceit.Id, sessionReceit.GroupSes.Id));
    }

    [HttpPost]
    public async Task<ActionResult<SessionReceitDto>> Create(int doctorId,int groupSessionId, CreateSessionReceitDto createSessionReceitDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");
        
        var groupSession = await _groupSessionsRepository.GetAsync(doctorId, groupSessionId);
        if (groupSession == null) return NotFound($"Couldn't find a group session with id of {groupSessionId}");

        var sessionReceit = new SessionReceit{Quantity = createSessionReceitDto.Quantity};
        sessionReceit.GroupSes = groupSession;
        sessionReceit.Time = DateTime.Now;
        sessionReceit.Price = groupSession.Price * createSessionReceitDto.Quantity;

        if (groupSession.Spaces >= createSessionReceitDto.Quantity)
        {
            groupSession.Spaces -= createSessionReceitDto.Quantity;
            await _groupSessionsRepository.UpdateAsync(groupSession);

            await _sessionReceitsRepository.CreateAsync(sessionReceit);

            return Created("GetReceit", new SessionReceitDto(sessionReceit.Id, sessionReceit.GroupSes.Id));
        }

        return BadRequest("Can't choose bigger quantity than available spaces.");
    }

    [HttpDelete("{sessionReceitId}")]
    public async Task<ActionResult> Remove(int doctorId, int groupSessionId, int sessionReceitId)
    {
        var sessionReceit = await _sessionReceitsRepository.GetAsync(doctorId, groupSessionId, sessionReceitId);
        if (sessionReceit == null)
            return NotFound();

        await _sessionReceitsRepository.RemoveAsync(sessionReceit);

        // 204
        return NoContent();
    }
}