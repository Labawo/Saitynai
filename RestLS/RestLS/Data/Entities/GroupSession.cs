﻿namespace RestLS.Data.Entities;

public class GroupSession
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Time { get; set; }
    public DateTime GSDate { get; set; }
    public int Spaces { get; set; }
    public float Price { get; set; }
    public Doctor Doc { get; set; }
}