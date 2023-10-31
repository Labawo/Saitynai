using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestLS.Auth;
using RestLS.Auth.Models;

namespace RestLS.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ClinicUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(UserManager<ClinicUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        var user = await _userManager.FindByNameAsync(registerUserDto.UserName);

        if (user != null)
            return BadRequest("This user already exists.");

        var newUser = new ClinicUser
        {
            Email = registerUserDto.Email,
            UserName = registerUserDto.UserName
        };

        var createUserResult = await _userManager.CreateAsync(newUser, registerUserDto.Password);

        if (!createUserResult.Succeeded)
            return BadRequest("Could not create a user.");

        await _userManager.AddToRoleAsync(newUser, ClinicRoles.Patient);

        return CreatedAtAction(nameof(Register), new UserDto(newUser.Id, newUser.UserName, newUser.Email));
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);

        if (user == null)
            return BadRequest("User with name or password does not exist.");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        
        if (!isPasswordValid)
            return BadRequest("User with name or password does not exist.");
        
        // valid user
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);

        return Ok(new SuccessfulLoginDto(accessToken));
    }
}