namespace RestLS.Data.Entities;

public class Appointment
{
    public int ID { get; set; }
    public DateTime AppointmentDate { get; set; }

    public Doctor Doc { get; set; }
}