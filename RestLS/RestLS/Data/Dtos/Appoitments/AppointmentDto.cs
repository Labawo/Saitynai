namespace RestLS.Data.Dtos.Appoitments;

public record AppointmentDto(int Id, DateTime Time, decimal Price, int DocId);
public record CreateAppointmentDto(decimal Price, string Time);
public record UpdateAppointmentDto(string Time, decimal Price);