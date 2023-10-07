namespace RestLS.Data.Dtos.Appoitments;

public record AppointmentDto(int Id, string Name);
public record AppointmentTimeDto(int Id, string Name, DateTime Time);
public record CreateAppointmentDto(string Name, double Price, string Time);
public record UpdateAppointmentDto(string Name, string Time, double Price);