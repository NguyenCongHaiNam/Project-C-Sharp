using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class NewsDetected
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Url { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(MAX)")]
    public string ResponseData { get; set; }

    [StringLength(100)]
    public string NegativeWords { get; set; }
}
