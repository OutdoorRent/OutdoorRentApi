using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Catalog.API.Data;
public enum ConditionEnum
{
    Excellent,
    Good,    
    Normal,    
    Bad   
}
public class Equipment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [ForeignKey("Location")]
    public string Location { get; set; }
    public Location LocationDetail { get; set; }
    public string Availability { get; set; }
    
    [Required(ErrorMessage = "equipment type id is required")]
    [ForeignKey("EquipmentType")]
    public int TypeId { get; set; }
    
    public EquipmentType Type { get; set; }
    
    [Required(ErrorMessage = "price is required")]
    public float PricePerDay { get; set; }
    public string Descriptions { get; set; }
    public ConditionEnum Condition { get; set; }
    public bool Deleted {get; set; }
}