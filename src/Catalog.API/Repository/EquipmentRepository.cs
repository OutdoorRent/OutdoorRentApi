using Catalog.API.Data;
using Catalog.API.Repository.Interfaces;

namespace Catalog.API.Repository;

public class EquipmentRepository: IEquipmentRepository
{
    private readonly MyDbContext _context;

    public EquipmentRepository(MyDbContext context) {
        _context = context;
    }

    public Equipment GetById(int id) => _context.Equipments.Find(id);

    public IEnumerable<Equipment> GetAll() => _context.Equipments.ToList();

    public void Add(Equipment product) {
        _context.Equipments.Add(product);
        _context.SaveChanges();
    }

    public void Update(Equipment product) {
        _context.Equipments.Update(product);
        _context.SaveChanges();
    }

    public void Delete(int id) {
        var product = _context.Equipments.Find(id);
        if (product != null) {
            _context.Equipments.Remove(product);
            _context.SaveChanges();
        }
    }
}