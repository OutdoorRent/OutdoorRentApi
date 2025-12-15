using Catalog.API.Data;
using Catalog.API.Repository.Interfaces;
using Catalog.API.Models;

namespace Catalog.API.Services;

public class EquipmentService
{
    private readonly IEquipmentRepository _repo;

    public EquipmentService(IEquipmentRepository repo) {
        _repo = repo;
    }

    public Equipment GetEquipment(int id) {
        return _repo.GetById(id);
    }
    
    public void AddEquipment(Equipment equipment) {
        _repo.Add(equipment);
    }

    public void UpdateEquipment(Equipment equipment)
    {
        _repo.Update(equipment);
    }

    public void DeleteEquipment(int id)
    {
        Equipment equipment = _repo.GetById(id);
        equipment.Deleted = true;
        _repo.Update(equipment);
    }
}