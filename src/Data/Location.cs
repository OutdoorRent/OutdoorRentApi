using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Data;

public class Location
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string LocationDetail { get; set; }
    [Required]
    [ForeignKey("User")]
    public int ManagerId { get; set; }
    public User Manager { get; set; }
}