using Microsoft.AspNetCore.Mvc;
using RestLS.Data.Dtos.GroupSessions;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api/doctors/{doctorId}/groupsessions")]
public class GroupSessionsController : ControllerBase
{
    private readonly IDoctorsRepository _doctorsRepository;
    private readonly IGroupSessionsRepository _groupSessionsRepository;
    private readonly IAppointmentsRepository _appointmentRepository;

    public GroupSessionsController(IGroupSessionsRepository groupSessionsRepository, IDoctorsRepository doctorsRepository, IAppointmentsRepository appointmentRepository)
    {
        _groupSessionsRepository = groupSessionsRepository;
        _doctorsRepository = doctorsRepository;
        _appointmentRepository = appointmentRepository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<GroupSessionDto>> GetMany(int doctorId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return null;
        
        var gsessions = await _groupSessionsRepository.GetManyAsync(doctor.Id);
        return gsessions.Select(o => new GroupSessionDto(o.Id, o.Name, o.Doc.Id));
    }

    // /api/topics/1/posts/2
    [HttpGet("{groupsessionId}", Name = "GetGroupSession")]
    public async Task<ActionResult<GroupSessionDto>> Get(int doctorId, int groupsessionId)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");
        
        var groupsession = await _groupSessionsRepository.GetAsync(doctor.Id, groupsessionId);
        if (groupsession == null) return NotFound();

        return Ok(new GroupSessionDto(groupsession.Id, groupsession.Name, groupsession.Doc.Id));
    }

    [HttpPost]
    public async Task<ActionResult<GroupSessionDto>> Create(int doctorId, CreateGroupSessionDto groupSessionDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");

        var groupsession = new GroupSession
        {
            Name = groupSessionDto.Name, Price = groupSessionDto.Price,
            Spaces = groupSessionDto.Spaces, Description = groupSessionDto.Description
        };
        groupsession.Time = DateTime.Parse(groupSessionDto.Time);
        groupsession.Doc = doctor;
        
        var existingGroupSession = await _groupSessionsRepository.GetAsync(doctor.Id, groupsession.Time);
        if (existingGroupSession != null)
        {
            return Conflict("Group session at this time already exists.");
        }
        
        var existingAppointment = await _appointmentRepository.GetAsync(doctor.Id, groupsession.Time);
        if (existingAppointment != null)
        {
            return Conflict("Appointment at this time already exists.");
        }

        await _groupSessionsRepository.CreateAsync(groupsession);

        return Created("GetGroupSession", new GroupSessionDto(groupsession.Id, groupsession.Name, groupsession.Doc.Id));
    }

    [HttpPut("{groupsessionId}")]
    public async Task<ActionResult<GroupSessionDto>> Update(int doctorId, int groupsessionId, UpdateGroupSessionDto updateGroupSessionDto)
    {
        var doctor = await _doctorsRepository.GetAsync(doctorId);
        if (doctor == null) return NotFound($"Couldn't find a doctor with id of {doctorId}");

        var oldGroupSession = await _groupSessionsRepository.GetAsync(doctorId, groupsessionId);
        if (oldGroupSession == null)
            return NotFound();

        //oldPost.Body = postDto.Body;
        oldGroupSession.Name = updateGroupSessionDto.Name;
        oldGroupSession.Spaces = updateGroupSessionDto.Spaces;
        oldGroupSession.Price = updateGroupSessionDto.Price;
        oldGroupSession.Description = updateGroupSessionDto.Description;
        

        if (oldGroupSession.Time != DateTime.Parse(updateGroupSessionDto.Time))
        {
            oldGroupSession.Time = DateTime.Parse(updateGroupSessionDto.Time);
            var existingGroupSession = await _groupSessionsRepository.GetAsync(doctor.Id, oldGroupSession.Time);
            if (existingGroupSession != null)
            {
                return Conflict("Group session at this time already exists.");
            }
        
            var existingAppointment = await _appointmentRepository.GetAsync(doctor.Id, oldGroupSession.Time);
            if (existingAppointment != null)
            {
                return Conflict("Appointment at this time already exists.");
            }
        }

        

        await _groupSessionsRepository.UpdateAsync(oldGroupSession);

        return Ok(new GroupSessionDto(oldGroupSession.Id, oldGroupSession.Name, oldGroupSession
            .Doc.Id));
    }

    [HttpDelete("{groupsessionId}")]
    public async Task<ActionResult> Remove(int doctorId, int groupsessionId)
    {
        var groupsession = await _groupSessionsRepository.GetAsync(doctorId, groupsessionId);
        if (groupsession == null)
            return NotFound();

        await _groupSessionsRepository.RemoveAsync(groupsession);

        // 204
        return NoContent();
    }
}