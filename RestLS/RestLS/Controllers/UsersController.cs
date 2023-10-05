/*using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RestLS.Data;
using RestLS.Data.Dtos.Doctors;
using RestLS.Data.Entities;
using RestLS.Data.Repositories;

namespace RestLS.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersRepository _usersRepository;

    public UsersController(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    
    // AutoMapper library for making remapping for sending to API
    [HttpGet(Name = "GetUsers")]
    public async Task<IEnumerable<UserDto>> GetMany()
    {
        var users = await _usersRepository.GetManyAsync();
        
        return users.Select(o => new UserDto(o.UserId, o.Nickname, o.Password));
    }
    
    
    
    // api/doctors/{doctorID}
    [HttpGet("{userId}", Name = "GetUser")]
    public async Task<ActionResult<UserDto>> Get(int userId)
    {
        var user = await _usersRepository.GetAsync(userId);
        if (user == null) return NotFound();

        return Ok(new UserDto(user.UserId, user.Nickname, user.Password));
    }
    
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto createUserDto)
    {
        var user = new User{Nickname = createUserDto.Nickname};
        user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(createUserDto.Password, 13);

        await _usersRepository.CreateAsync(user);
        
        //201
        return Created("", new UserDto(user.UserId, user.Nickname, user.Password));
        //return CreatedAtAction("GetDoctor", new { doctorId = doctor.Id }, new DoctorDto(doctor.Name, doctor.Lastname, doctor.Description));
    }
    
    [HttpPut]
    [Route("{userId}")]
    public async Task<ActionResult<UserDto>> Update(int userId, UpdateUserDto updateUserDto)
    {
        var user = await _usersRepository.GetAsync(userId);

        if (user == null)
        {
            return NotFound();
        }
        
        user.Nickname = updateUserDto.Nickname;
        
        await _usersRepository.UpdateAsync(user);

        return Ok(new UserDto(user.UserId, user.Nickname, user.Password));
    }
    
    [HttpDelete("{userId}", Name = "DeleteUser")]
    public async Task<ActionResult> Remove(int userId)
    {
        var user = await _usersRepository.GetAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        await _usersRepository.RemoveAsync(user);
        
        //204
        return NoContent();
    }
}*/