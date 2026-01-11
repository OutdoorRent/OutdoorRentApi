using System.ComponentModel.DataAnnotations;

namespace Identity.API.Data;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string CognitoSub { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [MaxLength(100)]
    public string FullName { get; set; }

    [MaxLength(20)]
    public string Phone { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.Customer;

    [Required]
    public UserStatus Status { get; set; } = UserStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public string ProfileImageUrl { get; set; }
    public string Notes { get; set; }
}

public enum UserRole
{
    Customer,
    Admin
}

public enum UserStatus
{
    Active,
    Suspended
}
