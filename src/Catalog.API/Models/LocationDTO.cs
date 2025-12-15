using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models;

public class LocationDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LocationDetail { get; set; }
    public int ManagerId { get; set; }
    public string ManagerName { get; set; }
}

public class CreateOrModifyLocationDTO
{
    public int? Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string LocationDetail { get; set; }
    [Required]
    public int ManagerId { get; set; }
}