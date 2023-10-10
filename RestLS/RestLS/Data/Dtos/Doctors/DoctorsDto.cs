namespace RestLS.Data.Dtos.Doctors;

public record DoctorDto(int Id, string Name, string LastName, string PhoneNumb, string Email);
public record CreateDoctorDto(string Name,string LastName, string PhoneNumb, string Email, int Experience);
public record UpdateDoctorDto(string Name,string LastName, string PhoneNumb, string Email);