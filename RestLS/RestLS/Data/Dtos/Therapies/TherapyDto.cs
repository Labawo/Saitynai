namespace RestLS.Data.Dtos.Therapies;

public record TherapyDto(int Id, string Name, string Description);
public record CreateTherapyDto(string Name, string Description, string? DoctorId);
public record UpdateTherapyDto(string Name, string Description);