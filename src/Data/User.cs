using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Data;
public enum UserRole
{
    Manager,
    Admin,
    Normal
}
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public int Id { get; set; }
    public int Age { get; set; }
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public required string Codehash { get; set; }
    [Required]
    public List<UserRole> Roles { get; set; } = new(){UserRole.Normal};
    [Required]
    public bool Deleted { get; set; }
}

