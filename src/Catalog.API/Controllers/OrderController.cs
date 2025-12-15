using AutoMapper;
using Catalog.API.Data;
using Catalog.API.Models;
using Catalog.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.API.Extensions;

namespace Catalog.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController: ControllerBase
{
    private readonly IMapper _mapper;
    private readonly MyDbContext _db;
    private readonly ILogger<OrderController> _logger;
    private readonly OrderMessageSender _sender;

    public OrderController(IMapper mapper, MyDbContext db, ILogger<OrderController> logger, OrderMessageSender sender)
    {
        _mapper = mapper;
        _db = db;
        _logger = logger;
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequestDto dto)
    {
        var conflict = await _db.InventoryBookings
            .AnyAsync(b =>
                b.EquipmentId == dto.EquipmentId &&
                b.Date >= dto.StartDate.Date &&
                b.Date <= dto.EndDate.Date
            );
        if (conflict)
        {
            throw new Exception("This equipment is not available in selected period.");
        }
        
        _logger.LogInformation("Received order creation request for equipment: {0}", dto.EquipmentId);
        await _sender.CreateOrderAsync(dto);
        return Ok(dto);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetOrderList([FromQuery] PagedQueryParams paras)
    {
        var data = await _db.Orders
            .Include(e=>e.User)
            .Include(e=>e.Equipment)
            .ToPagedResultAsync<Order>(paras.PageNumber, paras.PageSize);
        return Ok(data);
    }
}