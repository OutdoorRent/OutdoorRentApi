using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Data;

public enum OrderStatus
{
    Pending,
    Paid,
    Cancelled,
    Received,
    Returned,
    Completed,
}

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }
    [Required]
    [ForeignKey("Equipment")]
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public decimal TotalAmount { get; set; }
    public DateTime PaidAt { get; set; }
    public DateTime ReturnAt { get; set; }
    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}