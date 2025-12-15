using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Data;

public class InventoryBooking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [ForeignKey("Equipment")]
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
    public DateTime Date { get; set; }
    [Required]
    [ForeignKey("Order")]
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
}