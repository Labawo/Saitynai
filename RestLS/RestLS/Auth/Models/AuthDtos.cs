using System.ComponentModel.DataAnnotations;
using Azure.Identity;

namespace RestLS.Auth.Models;

public record RegisterUserDto([Required] string UserName, [EmailAddress][Required] string Email, [Required] string Password);

public record LoginDto(string UserName, string Password);
public record UserDto(string Id, string UserName, string Email);

public record SuccessfulLoginDto(string AccessToken, string RefreshToken);

public record RefreshAccessTokenDto(string RefreshToken);