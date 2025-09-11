using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserSessionHistory
{
    public string Username { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public DateTime RefreshTokenExpiry { get; set; }

    public DateTime AccessTokenExpiry { get; set; }

    public decimal Id { get; set; }

    public DateTime? LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public DateTime? SessionClearOn { get; set; }

    public string? SessionClearBy { get; set; }

    public string? Comments { get; set; }
}
