using AutoMapper;
using AutoMapper.QueryableExtensions;
using Catalog.API.Data;
using Catalog.API.Models;
using Catalog.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.API.Repository.Interfaces;

namespace Catalog.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EquipmentController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly MyDbContext _db;
    private readonly EquipmentService _equipmentService;
    
    public EquipmentController(IMapper mapper, MyDbContext db, EquipmentService equipmentService)
    {
        _mapper = mapper;
        _db = db;
        _equipmentService = equipmentService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetEquipList()
    {
        var equipmentDtos = await _db.Equipments
            .Where(c=>c.Deleted == false)
            .Include(e => e.LocationDetail)
            .ThenInclude(l => l.Manager)
            .ProjectTo<EquipmentDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(equipmentDtos);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEquipById(int id)
    {
        var equipment = _equipmentService.GetEquipment(id);
        var dto = _mapper.Map<EquipmentDTO>(equipment);
        if (equipment.Deleted)
        {
            throw new Exception("Equipment not exist");
        }
        else
        {
            return Ok(dto);
        }

        
    }
    [HttpPost]
    [Route("modify")]
    public async Task<IActionResult> AddEquipment([FromBody] AddOrModifyEquipmentDTO dto)
    {
        if (dto.Id == null)
        {
            Equipment equipment = new Equipment()
            {
                TypeId = dto.TypeId,
                Location = dto.Location,
                Condition = Enum.Parse<ConditionEnum>(dto.Condition, ignoreCase: true),
                Descriptions = dto.Descriptions,
                Availability = dto.Availability,
                Deleted = false,
            };
            _equipmentService.AddEquipment(equipment);
            return new OkObjectResult(equipment);
        }
        else
        {
            int id = dto.Id.Value;
            Equipment equip = _equipmentService.GetEquipment(id);
            equip.Availability = dto.Availability;
            equip.Descriptions = dto.Descriptions;
            equip.Condition = Enum.Parse<ConditionEnum>(dto.Condition, ignoreCase: true);
            equip.Deleted = false;
            equip.TypeId = dto.TypeId;
            _equipmentService.UpdateEquipment(equip);
            return new OkObjectResult(equip);
        }
        
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteEquipment([FromBody] DeleteEquipmentDTO dto)
    {
        int id = dto.Id;
        _equipmentService.DeleteEquipment(id);
        return Ok(id);
    }
}

