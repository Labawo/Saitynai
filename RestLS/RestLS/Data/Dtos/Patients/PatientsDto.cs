namespace RestLS.Data.Dtos.Patients;

public record PatientDto(int Id, string Name, string Lastname, string Phone);
public record CreatePatientDto(string Name, string Lastname, string Phone);
public record UpdatePatientDto(string Name, string Lastname, string Phone);