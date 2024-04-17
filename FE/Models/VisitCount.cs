using System;
using System.ComponentModel.DataAnnotations;

public class VisitCount
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int Count { get; set; }
}
