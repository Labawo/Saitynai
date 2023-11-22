using System.ComponentModel.DataAnnotations;
using RestLS.Auth.Models;

namespace RestLS.Data.Entities;

public class Therapy : IUserOwnedResource
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    [Required]
    public required string DoctorId { get; set; }
    
    public ClinicUser Doctor { get; set; }
    
}