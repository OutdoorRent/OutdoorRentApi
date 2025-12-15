using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Catalog.API.Data;
using Catalog.API.Models;

namespace Catalog.API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class UserController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly MyDbContext _db;
    public UserController(IMapper mapper, MyDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }
    // [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var users = await _db.Users
            .Where(u => !u.Deleted)
            .ToListAsync();
        var dto = _mapper.Map<List<UserDTO>>(users);
        return Ok(dto);
    }

    [HttpPost]
    [Route("create")]
    public IActionResult Create([FromBody] UserCreateDTO UserCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string CodeHash;
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(UserCreateDto.Code));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            CodeHash = builder.ToString();
        }


        User user = new User
        {
            Name = UserCreateDto.Name,
            Email = UserCreateDto.Email,
            Codehash = CodeHash,
            Age = UserCreateDto.Age,
            Deleted = false,
            Roles = UserCreateDto.Roles,
        };
        _db.Users.Add(user);
        _db.SaveChanges();
        return new OkObjectResult(true);
    }

    [HttpPost]
    [Route("delete")]
    public IActionResult Delete([FromBody] UserDeleteDTO UserDeleteDto)
    {
        var user = _db.Users.Find(UserDeleteDto.Id);
        if (user != null)
        {
            user.Deleted = true;
            _db.Entry(user).State = EntityState.Modified;
            _db.SaveChanges();
        }
        return Ok(true);
    }
    
    [HttpPost]
    [Route("grant")]
    public IActionResult GrantRole([FromBody] GrantRoleDTO dto)
    {
        var user = _db.Users.Find(dto.Id);
        if (user != null)
        {
            user.Roles = dto.Roles;
            _db.SaveChanges();
        }
        return Ok(true);
    }
}

