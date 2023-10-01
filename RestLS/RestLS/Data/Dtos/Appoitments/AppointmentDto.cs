namespace RestLS.Data.Dtos.Appoitments;

public record AppointmentDto(int Id, string Name);
public record CreateAppointmentDto(string Name);
public record UpdateAppointmentDto(string Name);