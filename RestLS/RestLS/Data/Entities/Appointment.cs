namespace RestLS.Data.Entities;

public class Appointment
{
    public int ID { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime Time { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public Therapy Therapy { get; set; }
}