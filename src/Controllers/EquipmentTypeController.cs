using AutoMapper;
using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class EquipmentTypeController: ControllerBase
{
    private readonly MyDbContext _db;
    private readonly IMapper _mapper;

    public EquipmentTypeController(MyDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    [HttpGet]
    [Route("type")]
    public async Task<IActionResult> GetAllType()
    {
        var types = await _db.EquipmentTypes.ToListAsync();
        return Ok(types);
    }
    [HttpGet]
    [Route("category")]
    public async Task<IActionResult> GetAllCategory()
    {
        var cats = await _db.EquipmentCategories.Include(c => c.EquipmentTypes).ToListAsync();
        var result = cats.Select(c => new EquipmentCategoryDTO
        {
            Id = c.Id,
            CategoryName = c.CategoryName,
            EquipmentTypes = c.EquipmentTypes.Select(t => new EquipmentTypeDTO
            {
                Id = t.Id,
                TypeName = t.TypeName
            }).ToList()
        }).ToList();
        return Ok(result);
    }

    [HttpPost]
    [Route("type")]
    public async Task<IActionResult> AddNewType([FromBody]EquipmentNewTypeDTO newType)
    {
        var categories = await _db.EquipmentCategories
            .Where(c => newType.CategoryIds.Contains(c.Id))
            .ToListAsync();
        
        EquipmentType record;
        
        if (newType.Id.HasValue)
        {
            record = await _db.EquipmentTypes
                .Include(e => e.EquipmentCategorys)
                .FirstOrDefaultAsync(e => e.Id == newType.Id.Value);

            if (record != null)
            {
                record.TypeName = newType.TypeName;
                record.EquipmentCategorys = categories;
                _db.EquipmentTypes.Update(record);
            }
            else
            {
                record = new EquipmentType
                {
                    TypeName = newType.TypeName,
                    EquipmentCategorys = categories
                };
                _db.EquipmentTypes.Add(record);
            }
        }
        else
        {
            record = new EquipmentType
            {
                TypeName = newType.TypeName,
                EquipmentCategorys = categories
            };
            _db.EquipmentTypes.Add(record);
        }

        await _db.SaveChangesAsync();
        return Ok(new {
            Id = record.Id,
            TypeName = record.TypeName,
            CategoryIds = record.EquipmentCategorys.Select(c => c.Id).ToList()
        });
    }
    
    [HttpPost]
    [Route("category")]
    public IActionResult AddNewCategory([FromBody]EquipmentNewCategoryDTO newCat)
    {
        var types = _db.EquipmentTypes
            .Where(i => newCat.TypeIds.Contains(i.Id))
            .ToList();
        EquipmentCategory record;
        if (newCat.Id.HasValue)
        {
            record = _db.EquipmentCategories.Include(c => c.EquipmentTypes).FirstOrDefault(c => c.Id == newCat.Id.Value);
            if (record != null)
            {
                record.CategoryName = newCat.CategoryName;
                record.EquipmentTypes = types;
            }
            else
            {
                record = new EquipmentCategory()
                {
                    CategoryName = newCat.CategoryName,
                    EquipmentTypes = types
                };
                _db.EquipmentCategories.Add(record);
            }
        }
        else
        {
            record = new EquipmentCategory()
            {
                CategoryName = newCat.CategoryName,
                EquipmentTypes = types
            };
            _db.EquipmentCategories.Add(record);
        }
        _db.SaveChanges();
        return Ok(new
        {
            Id = record.Id,
            CategoryName = record.CategoryName,
            TypeIds = record.EquipmentTypes.Select(t => t.Id).ToList(),
        });
    }

    [HttpPost]
    [Route("image")]
    public async Task<IActionResult> UploadImage([FromForm] EquipmentImageFormDTO form)
    {
        if (form.File == null || form.File.Length == 0)
            return BadRequest("文件为空");
        if (!Directory.Exists("Uploads"))
        {
            Directory.CreateDirectory("Uploads");
        }
        var filePath = Path.Combine("Uploads", form.FileName);
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            await form.File.CopyToAsync(stream);
        }
        return Ok("Upload success");
    }
}