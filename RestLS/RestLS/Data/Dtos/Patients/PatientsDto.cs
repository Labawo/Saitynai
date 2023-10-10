namespace RestLS.Data.Dtos.Patients;

public record PatientDto(int Id, string Name, string Lastname, string Phone, string Email);
public record CreatePatientDto(string Name, string Lastname, string Phone, string Email);
public record UpdatePatientDto(string Name, string Lastname, string Phone, string Email);