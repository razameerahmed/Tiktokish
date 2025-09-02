using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? RealName { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsVerified { get; set; }

    public string Role { get; set; } = null!;

    public DateTime? LastLoginAt { get; set; }

    public string? Locale { get; set; }

    public string? DevicePreference { get; set; }

    public string EcomStatus { get; set; } = null!;
}
