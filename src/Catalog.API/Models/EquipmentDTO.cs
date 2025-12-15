using System.ComponentModel.DataAnnotations;
using Catalog.API.Data;

namespace Catalog.API.Models;

public class EquipmentDTO
{
    public int Id { get; set; }
    public string Location { get; set; }
    public int ManagerId { get; set; }
    public string ManagerName { get; set; }
    
    public int TypeId { get; set; }
    public string TypeName { get; set; }
    public List<string> CategoryNames { get; set; }
    
    public string Availability { get; set; }
    public float PricePerDay { get; set; }
    public string Descriptions { get; set; }
    [EnumCaseInsensitiveValidation(typeof(ConditionEnum))]
    public string Condition { get; set; }
}

public class AddOrModifyEquipmentDTO
{
    public int? Id { get; set; }
    [Required]
    public string Location { get; set; }
    [Required]
    public int TypeId { get; set; }
    [EnumCaseInsensitiveValidation(typeof(ConditionEnum))]
    public string Condition { get; set; }
    public string Availability { get; set; }
    public float PricePerDay { get; set; }
    public string Descriptions { get; set; }
}

public class DeleteEquipmentDTO
{
    public int Id { get; set; }
}

public class EnumCaseInsensitiveValidationAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public EnumCaseInsensitiveValidationAttribute(Type enumType)
    {
        _enumType = enumType;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var valueAsString = value.ToString();

        if (Enum.TryParse(_enumType, valueAsString, true, out _))
        {
            return ValidationResult.Success;
        }

        // 如果无法转换，返回失败的验证结果
        return new ValidationResult($"Invalid value for enum {_enumType.Name}. The value '{valueAsString}' is not valid.");
    }
}