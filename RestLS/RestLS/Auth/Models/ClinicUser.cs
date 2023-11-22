using Microsoft.AspNetCore.Identity;

namespace RestLS.Auth.Models;

public class ClinicUser : IdentityUser
{
    public bool ForceRelogin { get; set; }
}