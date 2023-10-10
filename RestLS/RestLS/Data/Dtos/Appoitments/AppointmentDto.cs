namespace RestLS.Data.Dtos.Appoitments;

public record AppointmentDto(int Id, DateTime Time, double Price, int DocId);
public record CreateAppointmentDto(double Price, string Time);
public record UpdateAppointmentDto(string Time, double Price);