namespace Catalog.API.Models;

public class OrderCreateRequestDto
{
    public int UserId { get; set; }
    public int EquipmentId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
