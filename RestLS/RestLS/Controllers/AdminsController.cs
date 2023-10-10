using Microsoft.AspNetCore.Mvc;
using RestLS.Data.Dtos.Admins;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api/admins")]
public class AdminsController : ControllerBase
{
    private readonly IAdminsRepository _adminsRepository;

    public AdminsController(IAdminsRepository adminsRepository)
    {
        _adminsRepository = adminsRepository;
    }
    
    // AutoMapper library for making remapping for sending to API
    [HttpGet(Name = "GetAdmins")]
    public async Task<IEnumerable<AdminDto>> GetMany()
    {
        var admins = await _adminsRepository.GetManyAsync();
        
        return admins.Select(o => new AdminDto(o.Id, o.Name, o.Lastname));
    }
    
    
    
    // api/doctors/{doctorID}
    [HttpGet("{adminId}", Name = "GetAdmin")]
    public async Task<ActionResult<AdminDto>> Get(int adminId)
    {
        var admin = await _adminsRepository.GetAsync(adminId);
        if (admin == null) return NotFound();

        return Ok(new AdminDto(admin.Id, admin.Name, admin.Lastname));
    }
    
    [HttpPost]
    public async Task<ActionResult<AdminDto>> Create(CreateAdminDto createAdminDto)
    {
        var admin = new Admin{Name = createAdminDto.Name, Lastname = createAdminDto.Lastname};
        

        await _adminsRepository.CreateAsync(admin);
        
        //201
        return Created("", new AdminDto(admin.Id, admin.Name, admin.Lastname));
        //return CreatedAtAction("GetDoctor", new { doctorId = doctor.Id }, new DoctorDto(doctor.Name, doctor.Lastname, doctor.Description));
    }
    
    [HttpPut]
    [Route("{adminId}")]
    public async Task<ActionResult<AdminDto>> Update(int adminId, UpdateAdminDto updateAdminDto)
    {
        var admin = await _adminsRepository.GetAsync(adminId);

        if (admin == null)
        {
            return NotFound();
        }

        admin.Name = updateAdminDto.Name;
        admin.Lastname = updateAdminDto.Lastname;
        
        await _adminsRepository.UpdateAsync(admin);

        return Ok(new AdminDto(admin.Id, admin.Name, admin.Lastname));
    }
    
    [HttpDelete("{adminId}", Name = "DeleteAdmin")]
    public async Task<ActionResult> Remove(int adminId)
    {
        var admin = await _adminsRepository.GetAsync(adminId);

        if (admin == null)
        {
            return NotFound();
        }

        await _adminsRepository.RemoveAsync(admin);
        
        //204
        return NoContent();
    }
}