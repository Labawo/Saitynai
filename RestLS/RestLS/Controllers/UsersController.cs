using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestLS.Auth.Models;

namespace RestLS.Controllers;

[ApiController]
[Route("api")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ClinicUser> _userManager;
    
    public UsersController(UserManager<ClinicUser> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpGet]
    [Route("patients")]
    [Authorize(Roles = ClinicRoles.Doctor)]
    public IActionResult GetPatients()
    {
        var patients = _userManager.GetUsersInRoleAsync(ClinicRoles.Patient)
            .Result
            .Select(u => new UserDto(u.Id, u.UserName, u.Email))
            .ToList();

        return Ok(patients);
    }
    
    [HttpGet]
    [Route("users")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<IActionResult> GetUsers()
    {
        var allUsers = await _userManager.Users.ToListAsync(); // Fetch all users asynchronously

        var nonAdminUsers = allUsers
            .Where(u => !_userManager.IsInRoleAsync(u, ClinicRoles.Admin).Result) // Filter out Admin users
            .Select(u => new UserDto(u.Id, u.UserName, u.Email))
            .ToList();

        return Ok(nonAdminUsers);
    }

    [HttpGet]
    [Route("doctors")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public IActionResult GetDoctors()
    {
        var doctors = _userManager.GetUsersInRoleAsync(ClinicRoles.Doctor)
            .Result
            .Select(u => new UserDto(u.Id, u.UserName, u.Email))
            .ToList();

        return Ok(doctors);
    }
    
    [HttpDelete]
    [Route("deleteUser/{userId}")]
    [Authorize(Roles = ClinicRoles.Admin)] // Assuming only administrators can delete users
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return NoContent(); // 204 No Content - Successful deletion
        }
        // If the deletion was not successful, return the errors
        return BadRequest(result.Errors);
    }
    
    /*[HttpPut]
    [Route("validateUser/{userId}")]
    [Authorize(Roles = ClinicRoles.Admin)] // Assuming only administrators can update user parameters
    public async Task<IActionResult> ValidateUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Update user parameters
        user.IsVerified = true;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Ok("User parameters updated successfully.");
        }
        return BadRequest(result.Errors);
    }*/
}