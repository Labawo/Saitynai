namespace RestLS.Data.Entities;

public class SessionReceit
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public Patient Pat { get; set; }
    public GroupSession GroupSes { get; set; }
}