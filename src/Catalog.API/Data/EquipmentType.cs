using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Catalog.API.Data;

public class EquipmentType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string TypeName { get; set; }
    public ICollection<EquipmentCategory> EquipmentCategorys { get; set; }
}