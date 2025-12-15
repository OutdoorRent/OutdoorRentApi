namespace Catalog.API.Models;

public class EquipmentTypeDTO
{
    public int Id { get; set; }
    public string TypeName { get; set; }
}

public class EquipmentNewTypeDTO
{
    public int? Id { get; set; }
    public string TypeName { get; set; }
    public int[]? CategoryIds { get; set; } = Array.Empty<int>();
}

public class EquipmentCategoryDTO
{
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public List<EquipmentTypeDTO> EquipmentTypes { get; set; }
}

public class EquipmentNewCategoryDTO
{
    public int? Id { get; set; }
    public string CategoryName { get; set; }
    public int[] TypeIds { get; set; } = Array.Empty<int>();
}

public class EquipmentImageFormDTO
{
    public string FileName { get; set; }
    public IFormFile File { get; set; }
}