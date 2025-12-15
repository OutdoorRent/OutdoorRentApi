using AutoMapper;
using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationController: ControllerBase
{
    private readonly IMapper _mapper;
    private readonly MyDbContext _db;

    public LocationController(IMapper mapper, MyDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetLocationList()
    {
        var locations = await _db.Locations.Include(l => l.Manager).ToListAsync();
        var dto = _mapper.Map<List<LocationDTO>>(locations);
        return Ok(dto);
    }
    [HttpPost]
    public async Task<IActionResult> AddOrModifyLocation([FromBody] CreateOrModifyLocationDTO dto)
    {
        Location? loc;
        // create logic
        if (dto.Id == null)
        {
            loc = new Location()
            {
                Name = dto.Name,
                LocationDetail = dto.LocationDetail,
                ManagerId = dto.ManagerId,
            };
            _db.Locations.Add(loc);
        }
        // modify logic
        else
        {
            loc = await _db.Locations.FirstOrDefaultAsync(l => l.Id == dto.Id);
            if (loc == null)
            {
                throw new Exception("Location not exist.");
            }
            loc.ManagerId = dto.ManagerId;
            loc.Name = dto.Name;
            loc.LocationDetail = dto.LocationDetail;
            _db.Locations.Update(loc);
        }
        await _db.SaveChangesAsync();
        return new OkObjectResult(loc);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var location = await _db.Locations.FirstOrDefaultAsync(l => l.Id == id);
        if (location == null)
        {
            throw new Exception("Location not exist.");
        }
        _db.Locations.Remove(location);
        await _db.SaveChangesAsync();
        return new OkObjectResult(true);
    }

}