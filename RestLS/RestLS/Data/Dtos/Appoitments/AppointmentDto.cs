namespace RestLS.Data.Dtos.Appoitments;

public record AppointmentDto(int Id, string Name, int DoctorId);
public record CreateAppointmentDto(string Name);
public record UpdateAppointmentDto(string Name);