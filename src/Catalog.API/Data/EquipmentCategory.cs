using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Data;

public class EquipmentCategory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string CategoryName { get; set; }
    public ICollection<EquipmentType> EquipmentTypes { get; set; }
}