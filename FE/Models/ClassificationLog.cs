using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ClassificationLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public DateTime Time { get; set; }

    [Required]
    [StringLength(1000)] 
    public string Url { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(MAX)")] 
    public string ResponseData { get; set; }
}
