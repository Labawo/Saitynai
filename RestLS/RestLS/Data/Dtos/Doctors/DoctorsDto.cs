namespace RestLS.Data.Dtos.Doctors;

public record DoctorDto(int Id, string Name, string LastName);
public record CreateDoctorDto(string Name,string LastName, string Description);
public record UpdateDoctorDto(string LastName, string Description);