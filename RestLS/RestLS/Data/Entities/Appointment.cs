namespace RestLS.Data.Entities;

public class Appointment
{
    public int ID { get; set; }
    public string Name { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Time { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public Doctor Doc { get; set; }
    public Patient Pat { get; set; }
}