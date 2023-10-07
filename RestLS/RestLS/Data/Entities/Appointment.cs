namespace RestLS.Data.Entities;

public class Appointment
{
    public int ID { get; set; }
    public string Name { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime Time { get; set; }
    public double Price { get; set; }
    public bool IsAvailable { get; set; }
    public Doctor Doc { get; set; }
    //public Patient Pat { get; set; }
}